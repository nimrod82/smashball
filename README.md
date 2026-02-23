# SmashBall – Technical Test

**Unity version:** 2022.3.25f1
---

## Overview

SmashBall is a small 1v1 arcade prototype focused on gameplay clarity and clean architecture rather than feature volume.

The project centers around:

- Deterministic ball movement (custom integration and collision handling, independent of Unity Physics)
- Explicit round state management
- Clear separation between gameplay, input, and UI layers
- Lightweight decoupling through a minimal service registry
- Polished but contained feedback systems

The goal was to keep the code predictable, readable, and easy to build on.

---

## Architecture

### Service Registry

A simple static `Services` class is used to register and resolve core systems:

- `IRoundService`
- `IArenaBounds`
- `IInputService`
- `ICameraShake`
- `UIManager`

This keeps systems loosely coupled without introducing a heavier dependency framework. All major systems remain explicit and easy to trace.

---

### Round Flow

`RoundManager` is the authoritative game loop using a clear `RoundState` enum:
Menu, Serving, Playing, Smashed, GameOver


It is responsible for:

- Player instantiation
- Ball lifecycle
- Serve meter logic
- Score tracking
- Camera transitions
- Game over handling

Serve quality is driven by a ping-pong timer and directly influences ball speed and visual feedback.

---

### Ball System

`BallController` performs manual position integration in `FixedUpdate`.

Key characteristics:

- Custom boundary reflection (supports up to two bounce iterations per step)
- Deterministic velocity updates
- Progressive homing
- Grace period after strike to prevent immediate self-collision
- Continuous collision detection using segment-to-point distance (handled in `PlayerController`)

Gameplay logic does not rely on Unity’s built-in physics engine, to keep the behavior stable and predictable.

---

### Player System

`PlayerController` handles:

- Movement clamped to the player’s half of the arena
- Manual strike detection using a strike radius + tolerance band
- Strike quality calculation based on distance from an ideal radius
- Strike cooldown gating
- Camera feedback triggering

Strike quality directly affects:

- Ball speed
- Trail intensity
- Camera shake amplitude

---

### Input Abstraction

Input is abstracted via `IPlayerInput`.

Implementations:

- `HumanPlayerInput` (wraps `IInputService`)
- `BotInput`

The bot uses the same input contract as the human player and does not bypass gameplay systems.

---

### Camera & Feedback

- `CameraFollow` (clamped horizontal tracking)
- `CameraPoseController` (async pose transitions for serve/gameplay)
- `CameraShake` (Perlin-noise-based shake with damping)
- `BallTrailFX` driven by strike quality
- Async UI transitions (`GameOverUI`, `SmashedFeedbackUI`)

UI animations use `async/await` with cancellation tokens instead of coroutines for better control and readability.

---

## Assets Used

- Unity primitives
- TrailRenderer
- TextMeshPro
- Custom UI elements

No paid or external asset packs were used.

---

## Known Issues

- Bot behavior is intentionally simple (no movement logic or predictive positioning).
- Service registry is minimal and global by design.

---

## Time Spent

Approximately **10–11 hours total**.

| Area | Time |
|------|------|
| Project setup & initial arena | ~1h |
| Services, input & player movement | ~2h |
| Ball movement & reflection logic | ~2h |
| Serve system & basic UI | ~1.5h |
| Collision detection, scoring & game loop | ~2h |
| Camera pose transitions & shake | ~1h |
| Trail & final gameplay polish | ~1h |

Development followed an iterative pattern:

1. Core gameplay loop  
2. Ball stability and collision correctness  
3. Serve system and UI  
4. Feedback polish  

---

## AI Usage

AI tools (ChatGPT) were used selectively as an engineering assistant.

Main use cases:

- Challenging implementation decisions and exploring alternative approaches.
- Validating boundary integration and collision handling strategies.
- Reviewing code structure and readability.
- Accelerating debugging iterations.

Two contained systems were initially prototyped with AI assistance:

- **Ball boundary integration (`IntegrateWithBounce`)**  
  Alternative integration strategies were explored, and a multi-step bounce approach was adopted after evaluating precision and stability trade-offs. The final implementation was reviewed and adjusted manually.

- **CameraShake system**  
  A Perlin-noise-based shake was generated to bootstrap feedback quickly, then tuned manually to match gameplay feel.

All architectural decisions, state flow, and gameplay systems were designed and validated independently. AI was used as a productivity tool, not as a substitute for reasoning or ownership.

I am comfortable explaining and re-implementing any part of the codebase without AI assistance.
