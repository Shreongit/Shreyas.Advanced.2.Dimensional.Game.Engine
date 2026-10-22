# 📘 SA2DGE (  Stage by Stage Theory Notes ) 

These notes explain the main concepts behind the SA2DGE development stages in order. Each stage builds on the previous one: the runtime gives the engine a lifecycle, the platform layer connects it to Windows, input lets it read player actions, the graphics backend communicates with the GPU, and the renderer turns geometry and textures into visible pixels.

> **Scope note:** The available development-log details clearly describe Stages 2.1–2.6 and later work on sprite rendering, alpha blending, and UV selection. The exact heading and individual implementation details for Stage 4.1.1 are not present in the retrieved log excerpts, so the final sections explain the documented rendering concepts without inventing a specific Stage 4.1.1 feature.

---

## Stage 2.1 — Runtime Foundation

The runtime is the part of the engine that controls how a game starts, runs, and shuts down. SA2DGE’s core concepts include the `Engine`, `Application`, `Game`, and `GameLoop`. The application provides the entry point, the engine coordinates initialization and lifetime, and the game contains the game-specific behavior that the engine runs.

The **game loop** repeatedly processes events and input, updates the game, renders a frame, and presents it. Games are real-time programs, so the engine also needs to measure time. A frame delta is the time elapsed between frames; it can help make movement and updates independent of how fast frames are rendered. A fixed timestep updates simulation at a consistent interval, such as 60 updates per second, while rendering can run at a different rate. Clamping an unusually large frame delta helps prevent a pause or hitch from causing one enormous update.

The stage also establishes the idea of managing resources and shutdown deliberately. Systems that create native or GPU resources must eventually release them. A predictable lifecycle gives later systems a reliable place to initialize, update, and dispose of their resources.

**Main idea:** Before an engine can render a game, it needs a dependable way to start, run its loop, track time, and shut down.

## Stage 2.2 — Windows Platform Backend

The platform layer contains functionality that depends on the operating system. SA2DGE uses Win32 to create and manage its Windows application window, but the rest of the engine should work through a more general `Window` abstraction instead of depending on Win32 details everywhere.

This is an example of **abstraction**: the public engine-facing interface describes what a window can do, while the Windows backend describes how those operations are implemented. It keeps operating-system-specific code contained and makes the architecture easier to maintain.

The window also receives operating-system messages. These are translated into engine-level window events, such as closing, resizing, minimizing, maximizing, or restoring the window. The game loop processes these events and can pass them to `Game.OnWindowEvent()`. Resizing matters to graphics because the size of the window and the size of the graphics backbuffer must remain consistent.

One important bug involved handling an initial resize event that repeated the graphics resize even when the dimensions had not changed. Recreating or resizing graphics resources unnecessarily can invalidate rendering state. Comparing the requested dimensions with the current dimensions prevents redundant resize work.

**Main idea:** The platform backend translates operating-system behavior into a stable interface the rest of the engine can use.

## Stage 2.3 — Input Backend

The input system provides a consistent way for game code to query keyboard, mouse, and gamepad controls without directly calling Windows APIs. SA2DGE defines engine-level input types and an `IInputBackend` interface, while the Windows backend performs the platform-specific work.

Input state is normally checked across frames. The **current state** records whether a key or button is down now, while the **previous state** records its state during the preceding update. Comparing them allows three useful questions:

- `IsDown`: Is the key or button currently held?
- `IsPressed`: Did it change from up to down this frame?
- `IsReleased`: Did it change from down to up this frame?

This distinction is important. Movement may use `IsDown` because the player wants continuous movement while holding a key, while a jump or menu confirmation may use `IsPressed` to trigger only once per press.

For the mouse, the engine tracks buttons, cursor position, movement delta, and wheel movement. Mouse coordinates need to be interpreted relative to the application window when game code expects client-area coordinates. Wheel movement is treated as a per-frame change rather than a permanent accumulated event, so the engine resets the frame’s wheel delta after it has been consumed.

Gamepad input adds another device with its own buttons, sticks, and triggers. Stick axes are normalized to a range from `-1` to `1`, while triggers use a range from `0` to `1`. The backend also needs to account for connection and disconnection. A shared interface lets game code query these controls without knowing the platform API used underneath.

**Main idea:** The input abstraction hides device-specific details and turns raw device state into useful, consistent game actions.

## Stage 2.4 — Graphics Backend

The graphics backend is the layer that communicates with the graphics API. SA2DGE uses Direct3D 11 through Vortice. Keeping a graphics abstraction around the backend helps the rest of the engine express rendering operations without spreading low-level D3D11 details throughout the codebase.

A D3D11 **device** creates graphics resources, such as buffers, textures, shaders, and views. The **device context** issues commands that use those resources and configure the rendering pipeline. The **swap chain** manages the buffers used to present rendered frames to the window. A **render target** is the destination where drawing occurs, and a render-target view allows the pipeline to write to that target.

At a basic level, a frame is cleared to a known color, drawing commands are issued, and the finished frame is presented through the swap chain. Window resizing requires the backbuffer-related resources and viewport to be updated to match the new dimensions. Resource lifetimes matter: objects that depend on an old backbuffer must be released or recreated in the correct order.

The backend also introduces GPU resource management. Vertex buffers store vertex data, index buffers store references that describe how vertices form geometry, and textures store image data. A resource wrapper should validate its state where appropriate and dispose of the underlying graphics resource when it is no longer needed.

**Main idea:** The graphics backend owns the low-level connection between SA2DGE and D3D11, including resource creation, rendering state, and presentation.

## Stage 2.5 — Graphics Backend Fixes and Stabilization

Stage 2.5 focused on resolving bugs and stabilizing the graphics backend after its initial implementation. This kind of work is a normal part of building a rendering system: creating the right classes is not enough; their API usage, resource ownership, initialization order, and interactions must also be correct.

Graphics APIs often expose strict requirements about which resource types can be bound together, how parameters are represented, and when resources may be resized or disposed. A mismatch between the engine’s abstraction and the underlying API can cause compilation errors, invalid resource creation, warnings, or runtime rendering failures. Fixes in this stage prepared the backend for more advanced rendering rather than adding a completely separate rendering feature.

The important engineering lesson is that a backend should be tested in small steps. Device creation, swap-chain creation, resource creation and disposal, shader compilation, texture upload, resize, and shutdown can be checked independently. A small smoke test helps isolate the layer where a failure occurs.

**Main idea:** Stabilization makes the graphics foundation dependable enough to build the actual rendering pipeline on top of it.

## Stage 2.6 — D3D11 Basic Geometry Rendering

This stage connected the graphics resources and commands into a working draw path. The milestone was to make GPU-rendered geometry visible in the window, rather than only clearing the screen to a solid color.

A **vertex** contains data describing a point used to construct geometry. In the documented rendering setup, each vertex contains a three-component position (`Float3`) and a four-component color (`Float4`). Together these occupy 28 bytes per vertex: 12 bytes for position and 16 bytes for color. The vertex buffer stores these records on the GPU.

An **index buffer** stores indices into the vertex buffer. Instead of duplicating vertices for every triangle, the renderer can reuse them. Four vertices and six indices can describe two triangles that form a rectangle. The **vertex layout** or input layout tells Direct3D how to interpret the bytes in each vertex: where the position begins, where the color begins, and what types those attributes use.

Shaders are small programs that run at stages of the GPU pipeline. The **vertex shader** processes each input vertex and passes its attributes onward. The **pixel shader** determines the output color for the pixels covered by the geometry. Rasterization is the pipeline stage that converts triangle coverage into fragments/pixels to be processed by the pixel shader. Interpolation allows vertex colors to vary smoothly across the surface, producing a color gradient.

A vertex array groups the vertex buffer, index buffer, and input layout into a resource that can be bound together. The command path then binds the vertex array and shader program, calls `DrawIndexed`, resets or updates relevant state, and presents the result. This connects the high-level engine command API to the D3D11 pipeline.

Debugging the first black screen illustrates an important technique: test the rendering path one part at a time. A visible clear color proves that the window, render target, and presentation path are functioning. Then shader compilation, vertex stride, input layout, buffer contents, resource bindings, and draw commands can be checked individually. Once the colorful rectangle appeared, it demonstrated that geometry data had travelled through the pipeline and been rasterized by the GPU.

**Main idea:** Basic geometry rendering proves that SA2DGE can send structured vertex data and shaders to the GPU and produce drawn geometry on screen.

## Stage 4.1.1 — Rendering Foundation and Sprite Pipeline Context

The available log excerpts do not show the exact title or a distinct list of changes for Stage 4.1.1. The notes here therefore cover the documented rendering concepts that lead into sprite rendering, rather than claiming that a particular unverified feature was implemented in that stage.

A **renderer** provides a higher-level way to request drawing without requiring every game object to manage low-level GPU commands. A sprite renderer builds on the geometry pipeline: it supplies geometry for a 2D image, binds a texture and shader, configures the necessary state, and issues a draw command. The GPU then maps the image onto the geometry and produces pixels in the render target.

A texture is not automatically ready to use simply because image bytes exist in CPU memory. The engine must create a GPU texture, upload the pixel data, and create a shader-resource view so the shader can sample it. A sampler describes how the shader should read the texture, including filtering and behavior for coordinates outside the texture.

Texture **filtering** determines how sampled colors are calculated when texture pixels do not map exactly to screen pixels. Nearest filtering selects a nearby texel and can preserve a crisp pixel-art look. Linear filtering blends neighboring texels and can appear smoother. Texture **addressing** determines what happens when UV coordinates go beyond the usual `0` to `1` range. Repeat tiles the texture, mirrored repeat alternates its orientation, clamp-to-edge extends edge texels, and clamp-to-border uses a border value where supported.

**Main idea:** The sprite pipeline combines geometry, texture resources, sampler state, shaders, and draw commands to turn a 2D image into rendered output.

## Stage 4.2.1 — Alpha Blending Support

Alpha represents a texel’s opacity. An alpha value of `1` means fully opaque, `0` means fully transparent, and a value between them means partially transparent. Without an appropriate blending configuration, transparent parts of a sprite may render as solid blocks or fail to composite as intended.

Alpha blending combines the incoming source color with the color already stored in the render target. In a common source-alpha blend, source alpha controls how much of the new sprite color contributes, while the remaining contribution comes from the destination color. This allows a transparent-background image to appear over a scene instead of covering it with its background color.

For example, a fully opaque texel replaces the destination color, a fully transparent texel contributes none of its source color, and a semi-transparent texel combines both colors. Correct blending requires the graphics pipeline to use the intended blend state and the texture to contain meaningful alpha values.

**Main idea:** Alpha blending lets sprites with transparent and semi-transparent pixels composite correctly over previously rendered content.

## Stage 4.2.2 — Texture UV Selection

UV coordinates describe which part of a texture is mapped onto a piece of geometry. The `U` axis runs horizontally across the texture and the `V` axis runs vertically. UV coordinates are commonly normalized, so the full texture occupies a range from `0` to `1` on each axis.

A UV **offset** selects where the chosen region begins, while UV **size** specifies the width and height of the region. For example, selecting the upper-left quadrant means using an offset at the upper-left region’s starting coordinate and a size covering half the texture width and half its height, with the exact vertical convention depending on the texture coordinate orientation used by the renderer.

This is useful for sprite sheets and texture atlases, where many images or animation frames are stored inside one larger texture. The renderer can display a selected region without creating a separate texture for every sprite. UV selection changes which texels are sampled; it does not itself change the sprite’s world position, dimensions, or rotation.

The recorded validation checked all four texture quadrants and confirmed that UV selection worked with the existing sprite renderer without breaking rectangle rendering or sprite rotation.

**Main idea:** UV offset and size let one texture supply many different sprite regions while reusing the same rendering pipeline.

---

## How the Stages Fit Together

The runtime runs the application and coordinates updates. The platform layer supplies the window and operating-system events. The input backend translates device state into engine-level queries. The graphics backend creates and manages D3D11 resources, configures rendering state, issues commands, and presents frames. Geometry rendering proves that vertices and shaders can reach the screen; textures and UVs make that geometry display selected image content; alpha blending allows transparent and semi-transparent pixels to combine correctly with the scene.

# Stage 4.2.3 — Animation Playback Verification

## Objective

Verify that animation playback controls and frame timing behave as expected, and confirm that the engine initializes and shuts down cleanly while running the graphics pipeline.

## Core Concepts

- **Pause:** Suspends animation advancement while preserving the current frame, allowing playback to resume from the same point.
- **Stop:** Ends playback and resets the animation to its starting frame (`0`).
- **Non-looping playback:** Advances through the animation until the final frame, then stops instead of returning to the beginning.
- **Frame timing:** Animation advancement depends on elapsed time and the configured duration per frame. At `0.5` seconds per frame, a `1.1`-second update advances two frames, leaving `0.1` seconds.
- **Runtime lifecycle verification:** Successful Direct3D 11, graphics-pipeline, and shader initialization followed by clean shutdown confirms that the tested behavior runs within a functioning graphics runtime.

## Verification Results

| Test | Expected Behavior | Result |
|---|---|---|
| Pause | Preserve the current frame | Passed |
| Stop | Reset to frame `0` | Passed |
| Non-looping playback | Stop at frame `3` | Passed |
| Frame timing | Advance two frames during a `1.1`-second update at `0.5` seconds per frame | Passed |
| Runtime lifecycle | Initialize graphics resources and shut down cleanly | Passed |

## Outcome

The tested animation controls, non-looping behavior, and time-based frame advancement behaved as expected. The graphics runtime also initialized and shut down cleanly during verification.

This is why the stages build on one another. A sprite renderer is not just a single drawing function: it depends on a working runtime, a window, input and event handling where needed, a graphics device, valid resources, shaders, draw commands, texture sampling, and correct rendering state.

In SA2DGE, this functionality provides a foundation for **sprite sheets, animation frames, tile maps, and texture atlases**, where multiple images are stored in a single texture and selected using UV coordinates.


# Stage 4.2.4 — Sprite Animation Controls and Behaviour Verification

## Objective

Complete and verify the sprite animation system by testing playback controls, manual frame navigation, looping behaviour, and animation timing.

## Core Concepts

- **Sprite Flipping:** Horizontal and vertical flipping allow sprites to be mirrored without requiring separate texture assets.
- **UV Region Selection:** UV coordinates select specific regions of a texture or sprite sheet for rendering individual animation frames.
- **Automatic Frame Advancement:** The animation advances frames according to elapsed time and the configured frame duration.
- **Playback Controls:** Play, pause, stop, and restart manage the animation's playback state and position.
- **Looping:** Looping allows an animation to return to its beginning after reaching its final frame.
- **Manual Frame Control:** Left and right navigation allow individual frames to be selected and inspected directly.
- **Behaviour Verification:** Testing looping boundaries, timer reset, and restart behaviour helps confirm that animation state transitions work correctly.

## Verification Results

| Stage | Feature | Result |
|---|---|---|
| 4.2.1 | Horizontal and vertical sprite flipping | Verified |
| 4.2.2 | Sprite-sheet UV region selection | Verified |
| 4.2.3 | Automatic frame advancement and timing | Verified |
| 4.2.4.1 | Play, pause, stop, restart, and looping controls | Confirmed |
| 4.2.4.2 | Left/right manual frame navigation | Verified |
| 4.2.4.3 | Eight animation behaviour checks | All passed |

## Outcome

The sprite animation system's playback controls, manual frame navigation, looping boundaries, timer reset, and restart behaviour passed verification. This completes the current sprite animation implementation and testing stage.


# Stage 4.3 — Animation-to-Rendering Integration

## Objective

Integrate sprite animation with the rendering pipeline and verify that animation states, playback controls, and keyboard input work correctly at runtime.

## Core Concepts

- **Animation-to-Rendering Integration:** Connects animation frame selection to the rendering system so the appropriate sprite frame can be displayed.
- **Animation States:** Idle, Walk, and Run represent different animation states that can be selected through keyboard input.
- **Playback Control:** Pause, play, stop, and restart manage animation playback and its current state.
- **Frame Timing:** Controls how animation frames advance over elapsed time.
- **Looping:** Allows an animation to repeat after reaching its final frame.
- **Manual Frame Selection:** Enables direct selection of individual animation frames for testing and inspection.
- **Runtime Integration:** Successful project builds and Direct3D 11 startup confirm that the integrated systems can initialize and run together.

## Verification Results

| Test | Result |
|---|---|
| Both projects compile successfully | Passed |
| Direct3D 11 runtime startup | Passed |
| Pause and stop behaviour | Passed |
| Animation timing and looping | Passed |
| Manual frame selection and restart | Passed |
| Keyboard switching: Idle, Walk, Run | Verified |
| Keyboard playback controls | Verified |

## Outcome

The animation system and rendering runtime were successfully integrated and tested. Animation playback behaviour and keyboard-driven state switching between Idle, Walk, and Run were verified.


# Stage 4.5 — Animation-to-Rendering Integration

## Objective

Integrate sprite-sheet textures, pixel-art filtering, animation playback, and keyboard controls into the Direct3D 11 rendering pipeline.

## Core Concepts

- **Sprite Texture Loading:** Loads `Assault-Class.png` as the texture asset used to render the soldier sprite.
- **Nearest-Neighbor Filtering:** Preserves crisp pixel edges by sampling the nearest texel instead of blending neighbouring texels.
- **Sprite-Sheet Integration:** Connects the 5×7 sprite sheet to the rendering pipeline so individual animation frames can be selected and displayed.
- **Animation Playback:** Idle, Walk, and Crawl animations advance through their respective frames during runtime.
- **Playback Controls:** Play, pause, stop, restart, and looping controls manage animation playback behaviour.
- **Manual Frame Selection:** Allows individual frames to be selected directly for inspection and testing.
- **Keyboard Integration:** Enables runtime animation switching and playback control through keyboard input.
- **Rendering Validation:** Confirms that the sprite renders correctly and that existing animation tests continue to pass.

## Verification Results

| Feature | Result |
|---|---|
| Both projects build successfully | Passed |
| Direct3D 11 runtime startup | Passed |
| `Assault-Class.png` texture loading | Passed |
| Nearest-neighbor pixel-art filtering | Verified |
| 5×7 sprite-sheet integration | Verified |
| Idle, Walk, and Crawl animation playback | Passed |
| Keyboard animation and playback controls | Passed |
| Existing animation tests | Passed |
| Soldier sprite rendering | Verified |

## Outcome

Sprite animation is successfully integrated with the Direct3D 11 rendering pipeline. Texture loading, pixel-art filtering, animation playback, keyboard controls, and sprite rendering have been verified.

