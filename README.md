# 🎮 SA2DGE

## Shreyas's Advanced 2 Dimensional Game Engine

SA2DGE, short for **Shreyas's Advanced 2 Dimensional Game Engine**, is a personal 2D game engine being built from scratch in **C#** with the primary purpose of understanding how a game engine works internally rather than simply using an existing engine as a black box. The project focuses on learning and implementing the fundamental technologies that allow a game to communicate with the operating system, process player input, manage time and game updates, communicate with the GPU, organize graphics resources, render 2D content, manage game objects and scenes, and eventually provide higher-level systems that make building a complete game possible.

A 2D game engine can be thought of as the software layer that sits between a game and the underlying computer system. Instead of the game directly communicating with Windows, handling native window messages, reading hardware input, creating GPU resources, compiling shaders, and issuing graphics commands, the engine provides organized systems that perform these responsibilities and expose simpler interfaces to the game. A game can therefore focus on describing what it wants to happen while the engine handles the lower-level work required to make that happen.

At its core, SA2DGE follows the idea of separating the **game layer**, the **engine layer**, the **platform layer**, and the **hardware/API layer**. The game communicates with engine systems such as input, timing, rendering, resources, scenes, and physics, while the engine communicates with platform abstractions for things such as windows and input, and platform-specific implementations translate those abstractions into operating-system APIs such as Win32 and graphics APIs such as Direct3D 11. This separation allows the higher-level engine code to remain independent from many of the details of the operating system and graphics API.

The basic operation of SA2DGE begins when the application starts the engine and initializes the required systems. The engine creates and manages the runtime, establishes the platform window, initializes input and graphics, and then enters the game loop. During each iteration of the game loop, the engine processes operating-system events, updates input states, advances the game using the configured timestep, allows the game to update its logic, and then performs rendering through the graphics system before presenting the completed frame to the screen. This process continuously repeats until the application receives a shutdown request, after which the engine shuts down its systems and releases the resources it owns.

The platform layer is responsible for connecting SA2DGE to the operating system while keeping those operating-system details away from the rest of the engine. On Windows, this means the platform implementation communicates with the Win32 API to create and manage the native application window, process native window messages, detect resizing and other window events, and provide the engine with the information it needs without requiring game code to understand Win32 directly. The same principle is used for input, where Windows-specific keyboard, mouse, and gamepad APIs are translated into SA2DGE's own input abstractions.

The graphics side of the engine follows a similar separation. SA2DGE provides a graphics abstraction that represents operations such as creating GPU resources, clearing the framebuffer, selecting rendering resources, issuing draw commands, resizing graphics resources, and presenting frames, while the Direct3D 11 backend performs the actual communication with the GPU. This allows the engine's renderer to work with concepts such as vertex buffers, index buffers, vertex arrays, textures, shaders, and graphics commands without requiring every higher-level system to directly manipulate Direct3D 11 objects.

The rendering process eventually takes game data such as the position, size, texture, and transformation of a 2D object and converts that information into GPU-readable resources and commands. Geometry is stored in vertex and index buffers, vertex layouts describe how that data should be interpreted, shaders define how vertices and pixels are processed, textures provide image data, and graphics commands submit the required operations to the GPU. The GPU processes this information through the graphics pipeline and produces pixels in the render target, after which the swap chain presents the rendered image to the application window.

SA2DGE is being designed as a layered system because a game engine is not a single piece of software but a collection of cooperating subsystems. The runtime controls execution, the platform layer communicates with the operating system, the input system converts hardware input into engine-level state, the graphics system communicates with the GPU, the renderer turns game data into drawing commands, and higher-level systems such as scenes, entities, resources, physics, animation, audio, and UI build upon these foundations to provide the functionality required by an actual game.

The long-term purpose of SA2DGE is therefore not simply to produce a library that can draw sprites, but to build a complete understanding of the relationship between a game, an engine, an operating system, and computer hardware. The project is being developed from the low-level foundations upward so that each abstraction is backed by an understanding of what happens underneath it, allowing the engine to gradually move from runtime and platform infrastructure toward actual game-facing functionality.

SA2DGE is currently designed around **C# and .NET**, with **Windows and Win32** providing the initial platform layer and **Direct3D 11 through Vortice** providing the graphics backend. These technologies form the current implementation foundation, while the architecture is being kept sufficiently separated so that platform- and graphics-specific code can remain isolated from the higher-level engine systems wherever practical.

The project is intentionally being developed incrementally, with each subsystem being understood, implemented, connected to the runtime, tested, and refined before the engine moves further upward. The goal is to eventually reach a point where creating a 2D game with SA2DGE feels simple at the game level while the engine underneath it handles the complex work of window management, input, timing, graphics, resources, rendering, scenes, physics, audio, and other supporting systems.

**SA2DGE is ultimately an attempt to build a 2D game engine while simultaneously understanding the engineering principles behind one — from the moment the application starts, through the game loop and platform layer, through input and GPU communication, all the way to the final pixels appearing on the screen.**


## 🛠️ Development Path

SA2DGE is being developed from the lowest-level runtime foundations toward higher-level 2D game development systems. The development path begins with the core runtime and game loop, then moves through platform and window management, input, graphics and rendering, mathematics, resources, scenes and entities, ECS architecture, physics, animation, audio, UI, serialization, and eventually development tools and editor functionality. Each system is built on top of the foundations below it, allowing the engine to gradually evolve from a low-level runtime into a complete 2D game development framework.

**Development Path:**  
🎮 Game → ⚙️ Runtime → 🧠 Core → 🖥️ Platform → 🪟 Window → 🎮 Input → 🎨 Graphics → 🖌️ Renderer → 📐 Math → 📦 Resources → 🌍 Scene → 🧩 ECS → 💥 Physics → 🎞️ Animation → 🔊 Audio → 🖱️ UI → 💾 Serialization → 🛠️ Tools

---

## 💻 Technology Stack

SA2DGE is currently being developed primarily with **C# and .NET**, providing the main language, runtime, project system, and development environment for the engine. The initial platform target is **Windows**, with the native **Win32 API** being used for window creation, window messages, platform events, and Windows-specific input functionality. The graphics backend is built around **Direct3D 11**, with **Vortice** providing the .NET bindings that allow C# code to communicate with Direct3D 11 and DXGI. **XInput** is used for the initial Windows gamepad implementation, while the engine's own abstractions keep these platform-specific technologies separated from higher-level game and engine code.

**Current Stack:**  
`C#` → `.NET` → `Win32` → `Direct3D 11 / DXGI` → `Vortice` → `XInput`

---

## 📖 Development Log

The main README intentionally describes **what SA2DGE is, what it is designed to do, and how the engine works at a high level**, rather than documenting every implementation milestone. If you want to see what was built, changed, fixed, tested, and validated during each development stage, please refer to the **Development Log**, where the progress of SA2DGE is documented stage by stage it helps to understand the real work behind the project.
