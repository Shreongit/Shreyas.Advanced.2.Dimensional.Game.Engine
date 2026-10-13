# Contributing to SA2DGE

Thank you for your interest in contributing to **SA2DGE**!

SA2DGE is a 2D game engine focused on providing a simple, modular, and developer-friendly foundation for building 2D games. Contributions are welcome, whether they involve engine features, bug fixes, documentation, examples, tooling, or improvements to the development workflow.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Before You Start](#before-you-start)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Making Changes](#making-changes)
- [Engine Architecture Guidelines](#engine-architecture-guidelines)
- [Code Style](#code-style)
- [Commits](#commits)
- [Pull Requests](#pull-requests)
- [Bug Reports](#bug-reports)
- [Feature Requests](#feature-requests)
- [Documentation](#documentation)
- [Testing](#testing)
- [Performance Changes](#performance-changes)
- [Dependencies](#dependencies)
- [Breaking Changes](#breaking-changes)
- [Assets and Licensing](#assets-and-licensing)
- [Questions and Discussions](#questions-and-discussions)

---

## Code of Conduct

Please keep contributions respectful and constructive.

Contributors are expected to:

- Be respectful toward other contributors.
- Provide useful and constructive feedback.
- Focus discussions on the technical problem.
- Avoid personal attacks or harassment.
- Respect different approaches and experience levels.

---

## Before You Start

Before making a contribution:

1. Check existing issues and pull requests.
2. Make sure the issue has not already been addressed.
3. For large features, open an issue first to discuss the proposed design.
4. Keep changes focused on a specific problem or improvement.

For major engine architecture changes, discussion before implementation is strongly recommended.

---

## Getting Started

### 1. Fork the Repository

Fork the SA2DGE repository to your GitHub account.

### 2. Clone Your Fork

```bash
git clone https://github.com/<your-username>/SA2DGE.git
cd SA2DGE
```

### 3. Create a Branch

Create a dedicated branch for your work.

```bash
git checkout -b feature/my-feature
```

Use a descriptive branch name.

Examples:

```text
feature/sprite-rendering
feature/scene-system
fix/collision-detection
fix/window-resize
docs/getting-started
refactor/input-system
```

### 4. Build the Project

Follow the build instructions provided in the main `README.md`.

Make sure the project builds successfully before making changes.

### 5. Make Your Changes

Implement your changes while keeping the existing architecture and coding conventions in mind.

### 6. Test Your Changes

Run the available tests and verify that existing functionality has not been broken.

### 7. Commit Your Changes

```bash
git add .
git commit -m "Add sprite batching support"
```

### 8. Push Your Branch

```bash
git push origin feature/my-feature
```

### 9. Open a Pull Request

Create a Pull Request against the main SA2DGE repository.

---

## Project Structure

SA2DGE is organized into modular components.

The exact structure may evolve as the engine develops, but contributors should generally keep responsibilities separated.

For example:

```text
SA2DGE/
├── src/
│   ├── core/
│   ├── graphics/
│   ├── input/
│   ├── physics/
│   ├── audio/
│   └── ...
│
├── examples/
├── tests/
├── docs/
├── assets/
├── README.md
└── CONTRIBUTING.md
```

### General Principles

- Keep engine systems modular.
- Avoid unnecessary coupling between subsystems.
- Prefer clear interfaces between components.
- Avoid introducing dependencies without a strong reason.
- Keep platform-specific functionality isolated where possible.
- Do not mix unrelated changes in the same Pull Request.

---

## Development Workflow

SA2DGE follows a simple contribution workflow:

```text
Issue / Idea
     ↓
Discussion
     ↓
Implementation
     ↓
Testing
     ↓
Pull Request
     ↓
Review
     ↓
Merge
```

For larger features, contributors should first describe:

- What problem the feature solves.
- Why the feature belongs in the engine.
- Proposed API or architecture.
- Potential performance implications.
- Compatibility considerations.
- Testing strategy.

---

## Making Changes

When implementing a feature or fixing a bug, try to keep the change as small and focused as possible.

### Good

```text
Add collision filtering to physics system
```

### Avoid

```text
Add collision filtering + rewrite renderer + rename engine classes + update documentation
```

Unrelated changes make code review harder and increase the chance of introducing regressions.

---

## Engine Architecture Guidelines

When contributing to SA2DGE, keep the following principles in mind.

### Modularity

Each subsystem should have a clear responsibility.

For example:

```text
Renderer → Rendering
Input    → Input handling
Physics  → Collision and simulation
Audio    → Sound and music
Scene    → Game-world organization
Core     → Engine lifecycle and shared infrastructure
```

Avoid placing unrelated functionality inside a single system.

### Performance

SA2DGE is a game engine, so performance matters.

Consider:

- Avoiding unnecessary allocations in hot loops.
- Reducing redundant rendering operations.
- Avoiding unnecessary updates.
- Reusing resources where appropriate.
- Measuring performance before introducing complex optimizations.

Do not sacrifice readability for a micro-optimization without evidence that the optimization is necessary.

### API Design

Public engine APIs should be:

- Predictable.
- Consistent.
- Easy to understand.
- Minimal where possible.
- Stable unless a breaking change is intentional.

Before introducing a new public API, consider whether an existing abstraction can be extended instead.

---

## Code Style

Follow the existing coding style of the project.

General guidelines:

- Use meaningful names.
- Keep functions focused.
- Avoid unnecessary complexity.
- Prefer readable code over clever code.
- Document non-obvious behavior.
- Remove unused code and imports.
- Avoid unnecessary comments that simply restate the code.

### Example

Prefer:

```text
calculateCollisionResponse()
```

over:

```text
calcCR()
```

Names should communicate intent.

---

## Commits

Write clear and descriptive commit messages.

### Recommended Format

```text
<type>: <description>
```

Examples:

```text
feat: add sprite animation system
fix: prevent duplicate texture loading
refactor: simplify scene management
docs: update getting started instructions
test: add renderer tests
perf: reduce texture allocation overhead
```

Keep commits focused on one logical change whenever practical.

Avoid commit messages such as:

```text
stuff
changes
final
final final
fixed
update
```

---

## Pull Requests

Before submitting a Pull Request, make sure:

- [ ] The project builds successfully.
- [ ] Existing functionality still works.
- [ ] Tests pass.
- [ ] New functionality has appropriate tests where applicable.
- [ ] Documentation has been updated if necessary.
- [ ] No unnecessary files or generated artifacts are included.
- [ ] The Pull Request contains only related changes.
- [ ] Commit messages are clear.
- [ ] The Pull Request description explains the change.

### Pull Request Description

A good Pull Request should explain:

```text
## What changed?

Brief description of the implementation.

## Why?

Explain the problem this change solves.

## How?

Describe the important implementation details.

## Testing

Explain how the change was tested.

## Screenshots / Demo

Include screenshots, GIFs, or videos when the change affects
visual or gameplay behavior.
```

---

## Bug Reports

When reporting a bug, provide enough information to reproduce it.

Include:

- SA2DGE version or commit.
- Operating system.
- Compiler/runtime version if relevant.
- Steps to reproduce.
- Expected behavior.
- Actual behavior.
- Error messages or logs.
- Minimal reproduction when possible.
- Screenshots or videos for visual problems.

### Example

```text
## Bug

Sprite disappears when the window is resized.

## Steps to Reproduce

1. Start the example application.
2. Create a sprite.
3. Resize the window.
4. Observe the renderer.

## Expected Behavior

The sprite should remain visible.

## Actual Behavior

The sprite disappears after resizing.

## Environment

OS: Windows 11
SA2DGE: <commit/version>
```

---

## Feature Requests

Feature requests should explain the problem rather than only requesting a specific implementation.

Include:

- The problem you are trying to solve.
- Why the feature would be useful.
- Possible implementation approaches.
- Examples of how the API could be used.

For example:

```text
## Problem

Games currently have no built-in way to manage sprite animations.

## Proposed Solution

Introduce an animation component capable of managing
frames, frame duration, looping, and playback state.

## Example API

animation.play("run");
animation.pause();
animation.stop();
```

For large architectural changes, discuss the proposal before implementing it.

---

## Documentation

Documentation contributions are welcome.

You can contribute by improving:

- Installation instructions.
- API documentation.
- Tutorials.
- Examples.
- Architecture documentation.
- Comments explaining complex systems.
- Troubleshooting guides.

If a feature changes how users interact with SA2DGE, update the relevant documentation as part of the same Pull Request.

---

## Testing

Every contribution should be tested appropriately.

Depending on the change, testing may include:

- Unit tests.
- Integration tests.
- Example projects.
- Manual testing.
- Rendering verification.
- Performance testing.
- Platform-specific testing.

For engine systems, test both normal behavior and important edge cases.

Examples:

```text
Empty input
Invalid resource
Missing asset
Zero-sized object
Large number of entities
Window resizing
Repeated initialization
Repeated shutdown
```

---

## Performance Changes

If your contribution is intended to improve performance, provide evidence when possible.

Useful information includes:

- Benchmark results.
- Before/after measurements.
- Memory usage.
- Frame-time measurements.
- Number of objects tested.
- Hardware/environment used.

Avoid optimizing based only on assumptions.

---

## Dependencies

Avoid adding external dependencies unless they provide significant value.

Before adding a dependency, consider:

- Is it necessary?
- Can the functionality reasonably be implemented internally?
- Does it introduce platform-specific problems?
- What is its license?
- Is it actively maintained?
- What impact does it have on build size and complexity?

Discuss significant new dependencies before introducing them.

---

## Breaking Changes

Changes that modify or remove an existing public API should be treated carefully.

A Pull Request containing a breaking change should clearly explain:

1. What is changing.
2. Why the change is necessary.
3. What existing code will be affected.
4. How users can migrate.

Example:

```text
BREAKING CHANGE:

TextureManager::loadTexture() now returns a TextureHandle
instead of a raw Texture object.

Migration:

Old:
auto texture = loadTexture("player.png");

New:
auto texture = textureManager.load("player.png");
```

---

## Assets and Licensing

Only contribute assets that you have the right to distribute.

This includes:

- Textures.
- Sprites.
- Fonts.
- Audio.
- Models.
- Code from external projects.

Do not submit copyrighted assets without appropriate permission or licensing.

If an external asset is used, clearly document its license and source.

---

## Questions and Discussions

If you are unsure about an implementation, open an issue or discussion before making a large change.

Technical discussion is encouraged, especially around:

- Engine architecture.
- Rendering.
- Physics.
- Resource management.
- Performance.
- Public APIs.
- Platform support.

The goal is to build SA2DGE as a maintainable engine rather than simply adding features as quickly as possible.

---

## Final Notes

SA2DGE is an evolving project.

Not every contribution needs to be a major engine feature. Small improvements such as bug fixes, documentation, tests, examples, and tooling are valuable.

If you are contributing, focus on:

```text
Correctness
   ↓
Clarity
   ↓
Maintainability
   ↓
Performance
   ↓
Extensibility
```

Thank you for helping build SA2DGE , We are looking forward for this project! 🚀
