# Plan: Thesis Pattern Demonstrations

## Context

The thesis covers **8 design patterns** (in 8 chapters) for mobile game development. The Unity demo project (`Assets/Patterns Scenes/`) already has working code for all of them. Some need additions (Singleton variants, manual DI example) and the thesis needs a clear plan for which code snippets, figures, and screenshots to include per pattern. The mentor requires every pattern to follow an 8-point presentation format (problem, principle, pros/cons, performance, maintainability, use cases, implementation example, alternatives/mistakes).

**Final pattern list (nothing else gets its own discussion):**
1. Singleton
2. Observer
3. Command
4. State Machine
5. Object Pool
6. Flyweight
7. Decorator / Factory *(combined chapter — Factory reuses Decorator's class hierarchy)*
8. Dependency Injection / Service Locator

---

## Per-pattern thesis inclusion plan

### 1. Singleton
**Existing code:** `Assets/Patterns Scenes/5. Singleton/Scripts/` — 2 variants (classic C#, Unity MonoBehaviour).
**Code to add:** 3 more variants: thread-safe, generic `Singleton<T>`, scene-scoped.
**Code listings for thesis:**
- Listing: `SingletonCSharp.cs` — classic lazy singleton (full, ~56 lines)
- Listing: `SingletonUnity.cs` — MonoBehaviour + `DontDestroyOnLoad` + duplicate handling (full, ~82 lines)
- Listing: `SingletonThreadSafe.cs` — thread-safe with `Lazy<T>` (new, ~30 lines)
- Listing: `SingletonGeneric.cs` — generic `Singleton<T>` base class (new, ~40 lines)
- Listing: `SingletonSceneScoped.cs` — scene-scoped, no `DontDestroyOnLoad` (new, ~30 lines)

**Figures:**
- Comparison table: all 5 variants (columns: variant, initialization, thread safety, persistence, pros, cons)
- Diagram: Singleton lifecycle in Unity — `DontDestroyOnLoad` + scene reload duplicate problem

**Screenshot:** Demo scene console output showing both singleton references logging the same random number.

---

### 2. Observer
**Existing code:** `Assets/Patterns Scenes/3. Observer/` — 3 .cs files, 2 scenes.
**No code additions needed.**
**Code listings for thesis:**
- Listing: Trimmed excerpt from `DifferentEventAlternatives.cs` — the 4 main event type declarations + invocations side by side (~40 lines)
- Listing: `Enemy.cs` — static event declaration + `OnDisable` invoke (full, ~41 lines)
- Listing: `StaticEventsController.cs` — subscribe in `Awake()` + `AddToScore` handler (full, ~51 lines)

**Figures:**
- Comparison table: C# event mechanisms (EventHandler vs Action vs UnityEvent vs custom delegate — when to use, serializable?, parameter constraints)
- Sequence diagram: enemy dies → `OnDisable` → static event fires → controller's `AddToScore` runs

**Screenshot:** Console output showing kill count / score incrementing.

---

### 3. Command
**Existing code:** `Assets/Patterns Scenes/1. Command/` — 8 .cs files, 1 scene. Undo/redo/rebind/replay.
**No code additions needed.**
**Code listings for thesis:**
- Listing: `Command.cs` — abstract base with `Execute()`/`Undo()` (full, 16 lines)
- Listing: `MoveForwardCommand.cs` — concrete command (full, 31 lines)
- Listing: `DoNothingCommand.cs` — Null Object sub-pattern (full, 22 lines)
- Listing: Trimmed `GameController.cs` — `ExecuteNewCommand()`, undo/redo logic, `SwapKeys()` (~50 lines)

**Figures:**
- Class diagram: `Command` → `MoveForward`, `MoveBack`, `TurnLeft`, `TurnRight`, `DoNothing`
- Diagram: undo/redo stack flow (execute → push undo; undo → pop undo, push redo)

**Screenshot:** Game view showing object after moves + rebind keys demo.

---

### 4. State Machine
**Existing code:** `Assets/Patterns Scenes/6. State/Menu/` — 6 .cs files, 1 scene. Menu FSM with history stack.
**No code additions needed.**
**Code listings for thesis:**
- Listing: `_MenuState.cs` — abstract base (full, 32 lines)
- Listing: `MainMenu.cs` — concrete state with transition methods (full, 33 lines)
- Listing: Trimmed `MenuController.cs` — `SetActiveState()`, `JumpBack()`, dictionary + stack setup (~60 lines)

**Figures:**
- State transition diagram: Game ↔ Main Menu → Settings / Help (with Escape/back arrows)
- Class diagram: `_MenuState` → `MainMenu`, `SettingsMenu`, `HelpMenu`, `GameMenu`

**Screenshot:** Game view showing menu screens.

---

### 5. Object Pool
**Existing code:** `Assets/Patterns Scenes/18. Object Pool/Gun/` — 9 .cs files, 3 prefabs, 1 scene. Three implementations.
**No code additions needed.**
**Code listings for thesis:**
- Listing: `BulletObjectPoolSimple.cs` — linear search pool (full, 77 lines)
- Listing: Key excerpt from `BulletObjectPoolOptimized.cs` — `GetBullet()` + `ConfigureDeactivatedBullet()` (~50 lines)
- Listing: Key excerpt from `BulletObjectPoolUnity.cs` — `ObjectPool<T>` constructor + callbacks (~50 lines)

**Figures:**
- Comparison table: Simple (O(n)) vs Optimized (O(1) linked-list) vs Unity Native — complexity, code size, ease of use
- Diagram: linked-list pool (firstAvailable → bullet → bullet → null)
- Unity Profiler screenshot: GC Alloc per frame with/without pooling *(capture from running demo)*

**Screenshot:** Gun demo with bullets being fired and recycled.

---

### 6. Flyweight
**Existing code:** `Assets/Patterns Scenes/2. Flyweight/Scripts/` — 4 .cs files, 1 scene.
**No code additions needed.**
**Code listings for thesis:**
- Listing: `Data.cs` — shared intrinsic data, 20 doubles (full, 32 lines)
- Listing: `Heavy.cs` — no sharing, each instance creates own `Data` (full, 23 lines)
- Listing: `Flyweight.cs` — shared `Data` reference via constructor (full, 24 lines)
- Listing: `FlyweightController.cs` — creates 1M objects for comparison (full, 45 lines)

**Figures:**
- Memory diagram: Heavy (1M × Data) vs Flyweight (1M objects + 1 shared Data)
- Unity Profiler Memory screenshot: Heavy vs Flyweight side by side *(capture from running demo)*
- Table: memory breakdown (1M objects × 160 bytes/Data ≈ 153 MB difference)

**Screenshot:** Profiler memory view showing the difference.

---

### 7. Decorator / Factory *(combined chapter)*
**Why combined:** The Car Factory demo directly reuses the Decorator's `_Car`/`_CarExtras` class hierarchy — they are the same codebase demonstrating how Factory creates and Decorator wraps the same objects.

**Existing code:**
- Decorator: `Assets/Patterns Scenes/20. Decorator/Tesla order system/` — 9 .cs files, 1 scene
- Factory (Car): `Assets/Patterns Scenes/21. Factory/Car Factory/` — 4 .cs files, 1 scene (uses Decorator's classes)
- Factory (Sound): `Assets/Patterns Scenes/21. Factory/Sound Factory/` — 6 .cs files, 1 scene

**No code additions needed.**

**Code listings for thesis (Decorator half):**
- Listing: `_Car.cs` — abstract component with `GetDescription()` + `Cost()` (~15 lines)
- Listing: `Cybertruck.cs` — one concrete component example (~15 lines)
- Listing: `_CarExtras.cs` — abstract decorator base (~15 lines)
- Listing: `DracoThrusters.cs` — concrete decorator wrapping `_Car`, appending description + cost (~25 lines)
- Listing: Excerpt from `OrderSystemController.cs` — stacking decorators: `Roadster` + 5 thrusters + ejection seat (~20 lines)

**Code listings for thesis (Factory half):**
- Listing: `_CarFactory.cs` — abstract factory method (~15 lines)
- Listing: `USFactory.cs` or `ChinaFactory.cs` — one concrete factory showing region-specific constraints (~30 lines)
- Listing: `ISoundSystem.cs` — product interface (~10 lines)
- Listing: `SoundSystemFactory.cs` — simple/static factory with switch-based creation (~25 lines)

**Figures:**
- Class diagram: Decorator hierarchy (`_Car` → concrete cars + `_CarExtras` → concrete decorators)
- Class diagram: Factory hierarchy (`_CarFactory` → `USFactory` / `ChinaFactory`) using Decorator's products
- Diagram or table: how Factory creates what Decorator then wraps — showing the pattern interaction
- Table: Simple Factory (static method + switch) vs Factory Method (abstract class + subclasses) — when to use which

**Screenshots:** Console output from the order system (car + extras + cost), and the region-based factory (US vs China restrictions).

---

### 8. Dependency Injection / Service Locator
**Existing code:** `Assets/Patterns Scenes/15. Service Locator/` — 9 .cs files, 2 scenes.
**Code to add:** Simple manual DI example (constructor injection, no framework).
**Code listings for thesis:**
- Listing: `Audio.cs` — abstract service interface (~15 lines)
- Listing: `NullAudio.cs` — Null Object pattern for default service (~15 lines)
- Listing: `Locator.cs` — book-style audio service locator (full, 47 lines)
- Listing: `ServiceLocator.cs` — generic type-based locator (full, 35 lines)
- Listing: Manual DI example — constructor injection without framework (new, ~40 lines)

**Figures:**
- Diagram: Service Locator (pull — consumer asks locator) vs DI (push — dependencies injected at construction)
- Comparison table: Singleton vs Service Locator vs DI (coupling, testability, complexity, when to use)

**Screenshot:** Console output from audio service locator demo.

---

## Code additions summary

| What | File path | Why |
|---|---|---|
| Thread-safe Singleton | `.../5. Singleton/Scripts/SingletonThreadSafe.cs` | Mentor required variant |
| Generic `Singleton<T>` | `.../5. Singleton/Scripts/SingletonGeneric.cs` | Mentor required variant |
| Scene-scoped Singleton | `.../5. Singleton/Scripts/SingletonSceneScoped.cs` | Mentor required variant |
| Update GameController | `.../5. Singleton/Scripts/GameController.cs` | Exercise all 5 variants |
| Manual DI example | `.../15. Service Locator/Manual DI/` | Contrast with Service Locator |

---

## All figures/diagrams to create

| Type | Count | Patterns |
|---|---|---|
| Class diagrams | 5 | Command, State, Object Pool, Decorator, Factory |
| Sequence diagrams | 2 | Observer event flow, Command undo/redo |
| State transition diagram | 1 | State Machine menu flow |
| Memory/architecture diagrams | 4 | Flyweight layout, Object Pool linked-list, SL vs DI, Singleton lifecycle |
| Comparison tables | 7 | Singleton variants, Observer events, Object Pool impls, Flyweight memory, SL/DI/Singleton, Factory types, Decorator+Factory interaction |
| Profiler screenshots | 2 | Object Pool GC alloc, Flyweight memory |
| Demo screenshots | 8 | One per pattern chapter |

**Total: ~29 figures/tables/screenshots across 8 chapters.**

---

## Chapter order in thesis

1. **Увод** — why patterns matter (generally, then mobile-specific constraints)
2. **Singleton** — simplest, most common, sets up anti-pattern discussion early
3. **Observer** — event-driven foundation, mobile UI relevance
4. **Command** — encapsulated actions, undo/redo, builds naturally from Observer
5. **State Machine** — screen/menu flow, very visual, common mobile use case
6. **Object Pool** — performance-critical, strongest mobile argument
7. **Flyweight** — memory-critical, pairs with Object Pool for performance narrative
8. **Decorator / Factory** — combined chapter, Factory creates what Decorator wraps; shows pattern composition
9. **Service Locator / DI** — most architecturally advanced, wraps up with best practices
10. **Decision framework** — practical guidelines, comparison tables, decision diagram
11. **Закључак** — conclusion (past tense)

---

## Implementation steps (what Claude does)

1. **Update memory** — record final pattern list (8 patterns), update THESIS_REFERENCE.md
2. **Write Singleton variants** — `SingletonThreadSafe.cs`, `SingletonGeneric.cs`, `SingletonSceneScoped.cs`
3. **Update Singleton `GameController.cs`** — exercise all 5 variants
4. **Write manual DI example** — new folder under Service Locator with constructor injection demo
5. **Update THESIS_REFERENCE.md** — trim to only the 8 selected patterns, add Decorator/Factory section, remove irrelevant tiers

---

## Verification

- Open each pattern scene in Unity Editor and confirm it runs
- Verify all new Singleton variants compile and work
- Verify manual DI example compiles and runs
- Capture screenshots of each running demo
- Run Unity Profiler on Object Pool and Flyweight for benchmark screenshots
- Cross-check every pattern section covers all 8 points from standardized format
