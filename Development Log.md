## 📅 Stage 2.1 — Runtime Foundation Checkpoint

Today SA2DGE completed the core runtime foundation from **input through platform integration to graphics validation**. The input system was implemented with `InputKey`, `Keyboard`, `Input`, `IInputBackend`, and `WindowsInputBackend`, providing current/previous key states and `IsDown()`, `IsPressed()`, and `IsReleased()` behavior. Input is now integrated into the game loop, allowing the game to process keyboard input through the engine itself, including using `Input.Keyboard.IsPressed(InputKey.Escape)` to stop the runtime.

The runtime lifecycle was also fully integrated through **Engine → Application → GameLoop → Game**, with fixed 60 Hz updates, input processing, window event processing, rendering, shutdown, and resource cleanup. The Windows platform layer now handles native Win32 window creation, close, resize, minimize, maximize, restore, native handles, and window state tracking. The restore-event issue was fixed so restored windows correctly report their actual dimensions instead of `0x0`.

The **Direct3D 11 graphics backend** is now successfully integrated with the runtime. D3D11 device creation, flip-model swap chain creation, basic rendering/present, and shutdown were verified without duplicate initialization. The complete runtime smoke test successfully demonstrated the full path from engine startup through input, window events, D3D11 rendering, and clean it then ,shutdown.


## 📅 Stage 2.2 — Windows Platform Backend

Stage 2.2 focused on building and validating the **Windows platform backend** for SA2DGE. During this stage, the engine was connected to the native **Win32 API** through a platform-independent `Window` abstraction, allowing SA2DGE to create and manage a real Windows window without exposing Win32-specific code to the rest of the engine.

The window system now handles the complete native lifecycle, including creation, closing, resizing, minimizing, maximizing, restoring, and window events. Win32 messages are translated into SA2DGE `WindowEvent` objects and processed through the game loop, creating a clean boundary between the engine and the Windows operating system.

We also solved an important **Direct3D 11 resize issue** where the initial resize event could unnecessarily resize the graphics resources immediately after swap-chain creation. The graphics resize logic was corrected so that only actual dimension changes trigger `ResizeBuffers()` and render-target recreation.

Stage 2.2 was validated through live window resizing, continuous rendering, event propagation, window closing, and clean engine shutdown. This confirms that the **Windows platform layer and graphics resize path are now working together correctly**.

# 📅 SA2DGE — Stage 2.3

## 🎮 Input Backend — Complete

Stage 2.3 was focused on completing the input backend of SA2DGE so the engine can receive keyboard, mouse, and gamepad input through its own platform-independent input system instead of requiring game code to communicate directly with Windows APIs.

We completed the keyboard system by using the `InputKey` abstraction together with current and previous keyboard states, allowing SA2DGE to detect when a key is held with `IsDown()`, pressed with `IsPressed()`, or released with `IsReleased()`. The Windows backend uses `GetAsyncKeyState` to read keyboard state and supports letters, numbers, arrows, modifier keys, navigation keys, lock keys, and F1–F12, and the press, hold, and release behavior was validated at runtime.

We completed the mouse system by adding the `MouseButton` abstraction, current and previous button states, mouse position, movement delta, button press and release detection, and mouse-wheel input. The engine supports the left, right, middle, XButton1, and XButton2 buttons, while Windows `WM_MOUSEWHEEL` messages are converted into engine-level wheel input and the wheel delta is reset correctly each frame.

We also corrected the mouse coordinate system so that the engine uses coordinates relative to the active window's client area instead of global screen coordinates. This was achieved by using `ScreenToClient`, allowing mouse positions to correctly correspond to the game window.

We integrated the existing gamepad API with the Windows backend through XInput, allowing SA2DGE to detect controller connections and disconnections and read A, B, X, Y, shoulder, stick, Start, Back, and D-pad buttons, as well as both analog sticks and triggers. Analog stick values are normalized to approximately `-1.0` to `1.0`, while trigger values are normalized to `0.0` to `1.0`.

We updated `IInputBackend` so the active `Window` is available to the input backend, connected the input lifecycle to the game and window initialization process, added proper gamepad cleanup during shutdown and disconnection, connected native mouse-wheel messages to the input system, and fixed a `Math` namespace collision by using `global::System.Math.Clamp` for input normalization.

The final input flow is now Hardware → Windows Input Backend → SA2DGE Input System → Keyboard, Mouse, and Gamepad → Game, which keeps Windows-specific input implementation isolated from the rest of the engine.

The complete input system was rebuilt and runtime-tested, confirming that keyboard press, hold, and release states, mouse movement and buttons, window-relative mouse coordinates, mouse-wheel input, and gamepad input are working correctly.

Stage 2.3 is therefore officially complete, and SA2DGE now has a functional first version of its input layer that provides a clean foundation for future gameplay and engine systems.


# 🎨 SA2DGE — Stage 2.4: Graphics Backend

Stage 2.4 was focused on building the core graphics backend of SA2DGE and establishing the complete foundation required for the engine to communicate with the GPU and eventually perform real 2D rendering. Until this stage, SA2DGE had a working runtime, Windows platform layer, and input system, but the graphics side was still primarily concerned with creating the Direct3D 11 device, swap chain, and basic framebuffer presentation. The purpose of Stage 2.4 was to turn that initial graphics implementation into a proper engine-level graphics architecture where the rest of SA2DGE can request graphics operations through abstractions while Direct3D 11 remains an implementation detail underneath.

The first major part of this stage was the `GraphicsBackend` abstraction, which defines the interface between the engine and the actual graphics API. Instead of allowing the engine to directly depend on Direct3D 11 everywhere, the graphics system now provides controlled operations for initialization, resizing, clearing the framebuffer, presenting frames, creating graphics resources, creating shaders, creating textures, executing graphics commands, and safely disposing of graphics resources. This establishes an important architectural boundary because the higher-level engine can communicate with a graphics backend without needing to understand how D3D11 internally creates devices, buffers, textures, shader objects, or presentation resources.

The Direct3D 11 implementation was then expanded so that SA2DGE can communicate with the GPU through an actual D3D11 device and device context. The device represents the connection to the graphics hardware and is responsible for creating GPU resources, while the device context is responsible for configuring the graphics pipeline and submitting rendering commands. The existing swap-chain system was integrated into this architecture so that the engine can create a presentation surface connected to the native Windows window, render into the back buffer through a render target, and finally present the completed frame to the screen. The resize path was also connected to the graphics backend so that when the Windows window changes size, the engine can recreate the swap-chain buffers and render target and update the graphics viewport accordingly.

A major part of Stage 2.4 was introducing the GPU resource layer. We implemented abstractions for vertex buffers, index buffers, vertex arrays, and textures so that SA2DGE can now represent data that belongs on the GPU instead of treating all rendering information as CPU-side data. A vertex buffer stores vertex data that will eventually describe geometry, an index buffer stores indices that determine how that geometry is assembled, and a vertex array describes how the vertex data should be interpreted by the graphics pipeline. These abstractions are important because the future 2D renderer will use them to construct the geometry required to draw sprites and other objects.

The texture system was also significantly expanded during this stage. The engine's `Texture2D` abstraction was connected to an actual Direct3D 11 texture resource, allowing image data to be uploaded from CPU memory into GPU memory. A shader resource view was created so that the texture can later be accessed by shaders during rendering, while sampler states were created to control how the GPU samples texture data. The engine now supports filtering modes such as `Nearest` and `Linear`, as well as wrapping modes including `Repeat`, `MirroredRepeat`, `ClampToEdge`, and `ClampToBorder`. The texture system also handles the engine's `R8`, `RG8`, `RGB8`, and `RGBA8` formats, including conversion of three-channel `RGB8` data into four-channel `RGBA8` data where required by the selected Direct3D 11 texture representation. This means SA2DGE can now take texture data that exists in normal CPU memory and correctly create and upload a corresponding GPU texture resource.

Another important part of the graphics backend was the shader system. We implemented the ability for the graphics backend to create vertex and fragment shaders from source code, compile those shaders through the Direct3D 11 shader compiler, and combine the resulting shader objects into a usable shader program. The shader program represents the programmable portion of the graphics pipeline and will eventually determine how vertices are transformed and how pixels are produced. The engine can now create shader resources, attach the required shader stages, link them into a program, validate that the program is usable, and bind it before issuing draw commands. This establishes the programmable rendering foundation that the future 2D renderer will build upon.

The `GraphicsCommands` abstraction was also completed during this stage to prevent higher-level rendering code from directly manipulating the D3D11 device context. The command system provides operations for selecting a vertex array, selecting a shader program, performing indexed and non-indexed draw operations, resetting graphics state, and disposing of the command system. The Direct3D 11 implementation translates these engine-level commands into actual D3D11 operations such as `Draw()` and `DrawIndexed()`. This creates the beginning of the engine's rendering command layer, where the renderer will eventually describe what needs to be drawn while the graphics backend handles how those commands are translated into GPU operations.

We also added validation throughout the graphics system to prevent invalid states from silently reaching the GPU. Graphics operations are now protected against being performed before the graphics backend has been initialized, invalid dimensions are rejected, drawing without a selected vertex array or shader program is prevented, unlinked shader programs cannot be used, unavailable resources are detected, and disposed graphics objects cannot be reused. These checks are important because graphics APIs such as Direct3D 11 can fail in ways that are much harder to diagnose if invalid engine state is allowed to propagate deep into the rendering pipeline. By validating state at the engine abstraction level, SA2DGE can detect many incorrect operations closer to the source of the problem.

During development we also solved several integration and implementation problems that appeared while connecting the different graphics systems together. The Direct3D 11 texture implementation and inheritance structure had to be corrected to match the engine's actual `Texture2D` design, the texture constructor and data model had to be aligned with the backend implementation, D3D11 parameter conversions had to be corrected for unsigned values, shader-resource-view creation had to be updated to use the correct API pattern, and a namespace conflict involving `Math.Clamp` was resolved by explicitly referencing `global::System.Math.Clamp`. We also added the missing `GraphicsBackend.CreateTexture2D()` factory path so that textures can be created through the graphics abstraction rather than requiring the rest of the engine to know about the Direct3D implementation. Finally, the remaining nullable warning was removed so that the graphics implementation could build cleanly.

The graphics initialization and rendering flow now works as a complete chain rather than as isolated pieces. When SA2DGE starts, the graphics backend initializes the Direct3D 11 device and context, creates the swap chain for the engine window, obtains the back buffer, creates the render target, and configures the viewport. During the runtime, the game loop can ask the graphics backend to begin rendering, clear the render target, select graphics resources and shaders, issue drawing commands, and present the completed frame. If the window changes size, the platform layer produces a resize event, the graphics backend validates the new dimensions, resizes the swap-chain buffers, recreates the render target, and updates the viewport so rendering continues correctly at the new window dimensions.

The overall graphics flow at the end of Stage 2.4 can therefore be understood as the engine communicating with an abstract graphics interface, the graphics interface being implemented by Direct3D 11, the D3D11 backend managing the GPU device and resources, and the command system translating engine rendering operations into GPU commands. In simplified form, the pipeline is `Game → Graphics Backend → Graphics Commands → D3D11 Device Context → GPU → Swap Chain → Screen`, while resources such as `VertexBuffer`, `IndexBuffer`, `VertexArray`, `Texture2D`, and `ShaderProgram` are created through the graphics backend and supplied to the rendering commands when needed.

We also performed real runtime validation instead of relying only on successful compilation. The runtime successfully initialized the D3D11 device, created the flip-model swap chain, created and disposed graphics resources, compiled and linked D3D11 shaders, created a small `2×2` RGBA texture, uploaded the texture data to the GPU, disposed the texture correctly, processed window events and resizing, continued running the frame loop, and eventually reached a clean `SA2DGE shutdown`. This confirmed that the graphics backend is functioning as an integrated runtime subsystem rather than being only an interface or collection of untested classes.

In simple terms, Stage 2.4 changed SA2DGE from an engine that could merely create a graphics device and display a cleared framebuffer into an engine that now has the fundamental building blocks required for real GPU rendering. SA2DGE can create GPU buffers, describe vertex data, create textures, upload texture data, configure texture sampling, compile shaders, link shader programs, issue draw commands, resize graphics resources, present frames, validate graphics state, and safely dispose of everything when the runtime ends.

The most important result of this stage is that the **graphics foundation is now ready for the actual renderer**. We have not yet built the complete 2D rendering system, meaning the engine is not yet automatically turning sprites into visible textured quads, but all of the major low-level pieces required to do that now exist. The next stage can therefore focus on combining these pieces into a real rendering pipeline rather than continuing to build the underlying graphics infrastructure.

The next major goal is **Stage 2.5 — First Real 2D Renderer**, where SA2DGE will begin using the graphics backend to construct actual 2D geometry and send it through the shader pipeline. The renderer will build on the vertex buffers, index buffers, vertex arrays, shader programs, textures, and graphics commands completed in Stage 2.4, eventually allowing the engine to transform a simple sprite or textured quad from engine-level data into actual pixels displayed by the GPU.



## Stage 2.5 — Graphics Backend Fixes

Stage 2.5 focused on debugging, fixing, and validating bugs found in the Graphics Backend after Stage 2.4, ensuring the D3D11 foundation was stable and ready for actual rendering.



# SA2DGE — Stage 2.6 Development Log

Stage 2.6 was mainly focused on getting the first real GPU-rendered geometry working in SA2DGE and, more importantly, finding and solving the problems that were preventing the complete Direct3D 11 rendering pipeline from producing visible geometry.

The first major problem was that the engine could successfully create the window, initialize Direct3D 11, create the swap chain, compile the shaders, and start the runtime, but the screen still showed only the clear color instead of the expected geometry. This created the main debugging problem of the stage because the individual systems appeared to be working while the complete rendering path was not producing visible triangles.

We first verified that the problem was not related to the render target or presentation system by changing the clear color to a very obvious red color. The window successfully became red, proving that the D3D11 device, render target, clear operation, swap chain, and presentation path were working correctly. We then restored the normal dark-blue clear color and confirmed that the background continued to render correctly, allowing us to narrow the problem down to the actual geometry pipeline rather than the framebuffer or swap chain.

The next issue we investigated was whether the shaders were actually being created correctly. The vertex layout reported a stride of 28 bytes, which matched the vertex structure containing a `Float3` position and a `Float4` color, and the vertex and pixel shaders successfully compiled and linked. This ruled out shader compilation and basic vertex-layout creation as the cause of the missing geometry.

We then traced the actual rendering command path from the engine-level graphics API through `SetVertexArray()`, `SetShaderProgram()`, and `DrawIndexed()` into the Direct3D 11 graphics command implementation. This confirmed that the engine was no longer simply preparing graphics resources but was actually reaching the D3D11 device context and attempting to issue the indexed draw operation. The debugging therefore moved toward verifying the complete pipeline state required by the GPU for that draw call.

Another important problem we solved was establishing the correct relationship between the vertex buffer, index buffer, input layout, vertex shader, pixel shader, and vertex array. The four test vertices contained position and color information with a total stride of 28 bytes, while the index buffer contained six indices that formed two triangles. The input layout was configured so that the GPU could correctly interpret the first 12 bytes of each vertex as `POSITION` and the following 16 bytes as `COLOR`. This ensured that the data being sent to the GPU matched what the HLSL shaders expected.

We also implemented and connected the `VertexArray` abstraction so that the vertex buffer, index buffer, and input layout could be treated as a single rendering object. This prevented the runtime from having to manually manage every D3D11 binding and established the abstraction that the renderer can use for future geometry.

The shader pipeline was also completed for this test. The vertex shader accepted the `POSITION` and `COLOR` attributes and passed them through to the rasterizer, while the pixel shader returned the interpolated color as the final pixel output. Because the test positions were already in normalized device coordinates, no camera or projection matrix was required for this first geometry test. This kept the pipeline intentionally simple and allowed us to focus on validating the GPU rendering path itself.

We also verified the indexed drawing path and connected the engine-level `DrawIndexed()` command to the actual D3D11 `DrawIndexed()` call. This was an important architectural fix because the runtime does not directly access the D3D11 device context; instead, the request flows through the engine graphics abstraction into the Direct3D 11 backend and finally reaches the GPU.

The resize path was also verified during this work so that changing the native window dimensions does not leave the graphics pipeline using an invalid render target. When a resize event occurs, the old render target is released, the D3D11 context is flushed, the swap-chain buffers are resized, the new back buffer is obtained, a new render-target view is created, and the viewport is updated. This keeps the rendering surface synchronized with the actual window dimensions.

The main debugging process therefore consisted of isolating each part of the pipeline rather than changing everything at once. We first proved that clearing and presenting worked, then verified shader compilation and vertex-layout creation, then traced the draw command, and finally verified the relationship between the vertex data, index data, input layout, shader stages, vertex array, and D3D11 draw call. This allowed the black-screen problem to be narrowed down to the geometry rendering path instead of incorrectly changing the already-working window or swap-chain systems.

After these problems were solved, SA2DGE successfully reached the complete rendering path from CPU-side vertex data through the GPU pipeline. The engine can now create the vertex and index buffers, configure the input layout, compile and bind the vertex and pixel shaders, bind the vertex array, issue an indexed draw call, render the resulting triangles into the render target, and present the result through the swap chain.

The final result of today's work is that SA2DGE has crossed an important boundary: it is no longer only capable of initializing Direct3D 11 and displaying a cleared framebuffer, but can now issue a real GPU draw call and produce visible geometry inside the engine window. The colorful rectangle produced during the test is the first proof that the complete SA2DGE geometry-rendering pipeline is functioning.

**Stage 2.6 — D3D11 Basic Geometry Rendering: ✅ Complete**

**Next step:** build the higher-level 2D rendering functionality on top of this working GPU geometry pipeline.

**Next step:** build the higher-level 2D rendering functionality on top of this working GPU geometry pipeline.*
