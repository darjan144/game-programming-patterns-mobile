# Reference: Design & Architectural Patterns in Mobile Games

Working reference document for a *diplomski rad* (final thesis) on **mandatory design & architectural patterns in mobile games**. Source material: this repository (a Unity implementation of the patterns from Robert Nystrom's *Game Programming Patterns*, [gameprogrammingpatterns.com](http://gameprogrammingpatterns.com)), evaluated specifically for relevance to **mobile** game development (touch input, battery/thermal limits, constrained memory, GC-managed runtimes, app-store distribution, live-ops/F2P models, device fragmentation).

Each of the book's 23 patterns is placed into one of three tiers by how load-bearing it is for a typical mobile game's architecture. A fourth section extends beyond the book to patterns that are effectively mandatory in professional mobile projects but aren't covered by *Game Programming Patterns*. Code pointers reference files already implemented in this repo under [Assets/Patterns/](Assets/Patterns) and explanatory notes under [_text/](_text).

> Working thesis structure suggestion: use Tier 1 as the "core pattern" chapter, Tier 2 as "genre/scale-dependent patterns," Tier 3 as "patterns with limited mobile applicability" (useful as a *contrast* — showing you understand why some GPP patterns don't transfer to mobile), and Section 5 as "patterns beyond the book required in production mobile architectures."

---

## How tiers were decided

The classification criterion is **how often the pattern is architecturally necessary (not just possible) in a shipped mobile game**, weighted by mobile-specific constraints:

- **Memory/GC pressure** — mobile runtimes (Mono/IL2CPP, ART) pay heavily for allocation churn and GC pauses; frame drops are more visible on mobile (lower baseline headroom) → patterns that reduce allocation (Object Pool, Flyweight) rank higher than on PC/console.
- **Battery/thermal budget** — patterns that reduce redundant work (Dirty Flag) matter more.
- **Touch/UI-heavy, menu-heavy structure** — most mobile games spend disproportionate code on menus, popups, IAP/ad flows → State, Observer, Facade rank higher.
- **Small teams / live-ops / frequent content updates** — data-driven patterns (Type Object, Factory) rank higher because content teams need to add items/enemies without recompiling.
- **Genre skew** — mobile is dominated by casual/mid-core 2D or simple 3D games; patterns aimed at large open worlds, complex physics, or bytecode VMs for user modding (common in PC/console) are comparatively rare.

---

## Tier 1 — Core / mandatory patterns

Patterns that appear in the architecture of nearly every mobile game, regardless of genre.

### Update Method
- **Problem it solves:** each active game object needs its own per-frame behavior without hardcoding all of it into one giant loop.
- **Mobile relevance:** this *is* the Unity/Unreal/Godot component model's execution model. Every mobile game built on a commercial engine uses it implicitly; understanding it matters for **when to opt out** (e.g., batching updates manually for thousands of idle-game entities to avoid per-object `MonoBehaviour.Update` overhead, a very real mobile perf technique).
- **Code:** [_text/9-update-method.md](_text/9-update-method.md), custom implementation at [Assets/Patterns/9. Update/Custom Update method/Scripts](Assets/Patterns/9.%20Update/Custom%20Update%20method/Scripts) (`IUpdateable.cs`, `UpdateableComponent.cs`) — shows manually iterating a list instead of relying on the engine, which is exactly the technique used to reduce Update overhead in mobile idle/incremental games.
- **Pros:** simple, engine-supported. **Cons:** naive one-`MonoBehaviour`-per-object Update calls scale poorly past thousands of instances — a real mobile bottleneck.

### Game Loop
- **Problem it solves:** decoupling simulation rate from rendering/device speed.
- **Mobile relevance:** critical because of *device fragmentation* — the same game runs on wildly different hardware (low-end Android to high-end iOS). Fixed vs. variable timestep decisions directly affect determinism (important for replay/anti-cheat in competitive mobile games) and battery use (capping frame rate to 30/60 FPS is a common mobile battery-saving technique, unlike PC where uncapped is often the default).
- **Code:** [_text/8-game-loop.md](_text/8-game-loop.md) — Unity's `Time.deltaTime` / `Time.fixedDeltaTime` split.
- **Thesis angle:** good place to discuss `Application.targetFrameRate`, adaptive performance APIs (Android Adaptive Performance, iOS ProMotion throttling) as mobile-specific extensions of this pattern.

### Component
- **Problem it solves:** composition over inheritance; reusable, attachable behavior chunks.
- **Mobile relevance:** the foundational architectural pattern of every major mobile engine (Unity GameObject/Component, Unreal Actor/Component, Godot Node). Arguably **the** mandatory architectural pattern for the thesis's title claim.
- **Code:** [_text/13-component.md](_text/13-component.md) (no dedicated example folder — pattern is Unity's built-in `MonoBehaviour`/`GameObject` model; every other pattern folder in this repo implicitly demonstrates it).
- **Thesis angle:** compare classic Component (Unity GameObject) against data-oriented ECS (see Section 5) — many high-perf mobile titles now move from classic Component to ECS specifically for mobile CPU/battery reasons.

### Observer
- **Problem it solves:** decoupling the thing that triggers an event from the things that react to it.
- **Mobile relevance:** mobile games are UI/event-heavy (achievements, IAP success/fail callbacks, ad-watched callbacks, push-notification handling, score updates, daily-reward triggers). Nearly all of this is wired through C# `event`/`Action`/`UnityEvent` or platform SDK callbacks (billing, ads) which are Observer under the hood.
- **Code:** [_text/3-observer.md](_text/3-observer.md), [Assets/Patterns/3. Observer/Different events/DifferentEventAlternatives.cs](Assets/Patterns/3.%20Observer/Different%20events/DifferentEventAlternatives.cs) (compares `EventHandler`, `Action`, `UnityEvent`, custom delegate), [Assets/Patterns/3. Observer/Static events](Assets/Patterns/3.%20Observer/Static%20events) (global/static event bus).
- **Related/likely to discuss together:** Event Queue (Tier 2) as the "many events at once" fix; MVC/MVVM (Section 5) as the architectural pattern built on top of Observer.

### State
- **Problem it solves:** replacing deep conditional/branch logic with explicit, swappable state objects; FSMs.
- **Mobile relevance:** mobile games are dominated by screen/menu flow (splash → main menu → settings → gameplay → pause → results → shop) — this is almost always a state machine. Also standard for character/enemy AI and for game-mode states.
- **Code:** [_text/6-state.md](_text/6-state.md), [Assets/Patterns/6. State/Menu/Scripts/State](Assets/Patterns/6.%20State/Menu/Scripts/State) (`_MenuState.cs`, `MainMenu.cs`, `SettingsMenu.cs`, `HelpMenu.cs`, `GameMenu.cs`).
- **Thesis angle:** contrast hand-rolled FSM (this repo) vs. Unity Animator-driven state machines vs. visual scripting state graphs — all common in mobile production.

### Object Pool
- **Problem it solves:** avoiding runtime `Instantiate`/`Destroy` churn.
- **Mobile relevance:** arguably the **single most mobile-critical pattern** in the whole list. Mobile runtimes use managed GC (Mono/IL2CPP + Boehm-style or incremental GC); allocation spikes cause GC pauses that are far more visible on mobile (weaker CPUs, thermal throttling, no frame headroom) than on PC/console. Any mobile game with bullets, particles, floating combat text, or pooled UI list cells needs this.
- **Code:** [_text/18-object-pool.md](_text/18-object-pool.md), three implementations for direct comparison in [Assets/Patterns/18. Object Pool/Gun/Object pools](Assets/Patterns/18.%20Object%20Pool/Gun/Object%20pools): `Simple/` (naive list search), `Optimized/` (linked-list), `UnityNative/` (Unity's built-in `UnityEngine.Pool.ObjectPool<T>`).
- **Thesis angle:** this is a great pattern to actually **benchmark** (frame time / GC alloc per frame) on a real device for empirical thesis data — pooled vs. non-pooled bullet spawning is a clean, reproducible experiment.

### Singleton
- **Problem it solves:** guaranteed single instance + global access (GameManager, AudioManager, SaveManager).
- **Mobile relevance:** extremely common in real mobile codebases (`GameManager.Instance`, `AudioManager.Instance`, `AdsManager.Instance` patterns are everywhere in shipped mobile titles and asset-store templates) — but also the most **debated** pattern in the book itself. Worth treating as "mandatory in practice, discouraged in theory" — good thesis material on the gap between textbook best practice and shipped-code reality.
- **Code:** [_text/5-singleton.md](_text/5-singleton.md), [Assets/Patterns/5. Singleton/Scripts](Assets/Patterns/5.%20Singleton/Scripts) (`SingletonCSharp.cs` plain C#, `SingletonUnity.cs` MonoBehaviour variant with the double-instance/`OnDestroy` teardown problem called out).
- **Thesis angle:** the source text lists concrete alternatives (no class, static class/Service Locator, dependency injection, "one singleton") — useful as a comparison table against Service Locator and DI (Section 5).

### Service Locator
- **Problem it solves:** global access to swappable services (audio, input, localization, analytics) without hard-coding concrete types.
- **Mobile relevance:** mobile games integrate many interchangeable/platform-dependent services — ad mediation (AdMob vs. AppLovin vs. Unity Ads), IAP backends (Google Play Billing vs. Apple StoreKit), analytics (Firebase vs. GameAnalytics), push notifications, cloud saves. Abstracting these behind locatable service interfaces is close to mandatory once a game ships on both iOS and Android with multiple SDK vendors.
- **Code:** [_text/15-service-locator.md](_text/15-service-locator.md), [Assets/Patterns/15. Service Locator/Audio service locator/Scripts/Service Locator](Assets/Patterns/15.%20Service%20Locator/Audio%20service%20locator/Scripts/Service%20Locator) (`Locator.cs`, `Audio.cs`, `NullAudio.cs` — note the **Null Object** sub-pattern used to avoid null checks), and a second variant in [Assets/Patterns/15. Service Locator/Another Implementation](Assets/Patterns/15.%20Service%20Locator/Another%20Implementation).
- **Related:** compare against Dependency Injection frameworks used in production mobile (Zenject/Extenject, VContainer) — Section 5.

### Factory
- **Problem it solves:** centralizing object-creation logic, especially when creation varies by platform/config.
- **Mobile relevance:** used for spawning enemies/items/collectibles from data, and specifically useful for **platform-branching construction** (different ad SDK wrapper on iOS vs Android, different save serializer, different sound backend) — the "Sound Factory" example in this repo is directly analogous to mobile platform-abstraction code.
- **Code:** [_text/21-factory.md](_text/21-factory.md), [Assets/Patterns/21. Factory/Sound Factory](Assets/Patterns/21.%20Factory/Sound%20Factory) (`SoundSystemFactory.cs` picking hardware/software/other sound system), [Assets/Patterns/21. Factory/Car Factory](Assets/Patterns/21.%20Factory/Car%20Factory) (region-based factory variants — same shape as region/store-based branching, e.g. China vs. Global SDK builds).

### Flyweight
- **Problem it solves:** sharing heavy, non-instance-specific data across many lightweight instances.
- **Mobile relevance:** directly maps to two extremely common mobile optimizations: **texture atlases / sprite sheets** (fewer draw calls = fewer batches = better mobile GPU performance) and **shared mesh/material** reuse for large numbers of similar objects (crowds, tile-based levels, bullet-hell enemies). Given mobile GPUs are bandwidth- and draw-call-sensitive, this is close to mandatory for any game rendering many similar sprites/objects.
- **Code:** [_text/2-flyweight.md](_text/2-flyweight.md), [Assets/Patterns/2. Flyweight/Scripts](Assets/Patterns/2.%20Flyweight/Scripts) (`Flyweight.cs` shared data vs `Heavy.cs` /instance-specific data split).

---

## Tier 2 — Situational / genre- or scale-dependent

Common and useful, but not present in *every* mobile game — their necessity depends on genre (strategy/RPG vs. hyper-casual), scale (team size, content volume), or specific technical needs (networking, live-ops).

| Pattern | When it becomes relevant on mobile | Code pointer |
|---|---|---|
| **Type Object** | Data-driven content (enemy/item/ability definitions) so designers add content without new code/recompiles — very relevant wherever a game uses Unity `ScriptableObject`-based data (extremely common in live-ops mobile games with frequent content drops). | [_text/12-type-object.md](_text/12-type-object.md), [Assets/Patterns/12. Type Object/Animal/Scripts](Assets/Patterns/12.%20Type%20Object/Animal/Scripts) |
| **Command** | Ability/input systems, replay systems, undo (level editors, puzzle games), and encapsulating network-serializable actions in multiplayer mobile games. | [_text/1-command.md](_text/1-command.md), [Assets/Patterns/1. Command/Rebind keys](Assets/Patterns/1.%20Command/Rebind%20keys) |
| **Event Queue (Command Queue)** | Spreading many simultaneous events (e.g., 50 enemies dying at once) across frames to avoid a hitch — directly relevant to mobile frame-time budgets; also used for deferred asset loads (playing a sound a few frames after a tap so the UI doesn't stutter). | [_text/14-event-queue.md](_text/14-event-queue.md), [Assets/Patterns/14. Command Queue (Event Queue)](Assets/Patterns/14.%20Command%20Queue%20%28Event%20Queue%29) |
| **Dirty Flag** | Avoiding redundant costly work: re-saving unchanged game state, re-laying-out unchanged UI, skipping network sync for unchanged entities in multiplayer — a real battery/bandwidth saver on mobile. | [_text/17-dirty-flag.md](_text/17-dirty-flag.md), [Assets/Patterns/17. Dirty Flag/Unsaved changes](Assets/Patterns/17.%20Dirty%20Flag/Unsaved%20changes) |
| **Decorator** | Stacking modifiers: power-ups, equipment/gear attachments, temporary buffs — common in RPG/idle/gacha mobile genres. | [_text/20-decorator.md](_text/20-decorator.md), [Assets/Patterns/20. Decorator/Tesla order system](Assets/Patterns/20.%20Decorator/Tesla%20order%20system) |
| **Facade** | Wrapping messy third-party SDKs (ads, IAP, analytics) behind one clean interface — very common in real mobile codebases so the rest of the game doesn't depend directly on a specific ad/analytics vendor. | [_text/22-facade.md](_text/22-facade.md), [Assets/Patterns/22. Facade/Random numbers](Assets/Patterns/22.%20Facade/Random%20numbers) |
| **Template Method** | Shared multi-step algorithms with per-subtype variation: character ability sequences, level-generation skeletons, onboarding/tutorial step flows. | [_text/23-template.md](_text/23-template.md), [Assets/Patterns/23. Template/Assemble cars](Assets/Patterns/23.%20Template/Assemble%20cars) |
| **Subclass Sandbox** | Shared "toolbox" of protected methods for related behaviors (ability/superpower systems) — reduces duplication vs. many small unrelated classes. | [_text/11-subclass-sandbox.md](_text/11-subclass-sandbox.md), [Assets/Patterns/11. Subclass Sandbox/Superpowers](Assets/Patterns/11.%20Subclass%20Sandbox/Superpowers) |
| **Prototype** | Runtime cloning (Unity `Instantiate`) is universal, but the pattern is only *architecturally interesting* when combined with Object Pool/Factory to avoid the instantiate/destroy cost. | [_text/4-prototype.md](_text/4-prototype.md), [Assets/Patterns/4. Prototype/Monster spawner](Assets/Patterns/4.%20Prototype/Monster%20spawner) |
| **Spatial Partition** | Matters once a mobile game has many moving/colliding entities (tower defense, RTS-lite, battle-royale, large crowds) — CPU-bound collision/pathfinding checks are expensive on mobile hardware. Not needed for most casual/puzzle titles with few active entities. | [_text/19-spatial-partition.md](_text/19-spatial-partition.md), [Assets/Patterns/19. Spatial Partition/Grid](Assets/Patterns/19.%20Spatial%20Partition/Grid) |

---

## Tier 3 — Rarely load-bearing on mobile

These are legitimate, well-explained patterns in the book, but they solve problems that are uncommon or largely absent in typical mobile game production. Good for the thesis as **contrast material** — showing awareness of the full pattern catalogue while justifying why the "mandatory" set is smaller on mobile than on PC/console.

- **Double Buffer** — [_text/7-double-buffer.md](_text/7-double-buffer.md), [Assets/Patterns/7. Double Buffer/Cave](Assets/Patterns/7.%20Double%20Buffer/Cave). Frame double-buffering is handled entirely by the OS/GPU driver on mobile; the "roll-your-own" use case (cellular automata for cave/water/fire generation) is a niche indie-simulation technique, uncommon in mainstream mobile genres.
- **Bytecode** — [_text/10-bytecode.md](_text/10-bytecode.md), [Assets/Patterns/10. Bytecode/Scripts](Assets/Patterns/10.%20Bytecode/Scripts). Custom scripting VMs for modding/dialogue are rare in mobile because (a) most mobile stores/engines discourage or restrict runtime-loaded code for review/security reasons, and (b) small mobile teams rarely build in modding support. Where similar needs exist (dialogue trees, live-ops rules), teams more often reach for data-driven Type Object/ScriptableObjects or a small embedded scripting library rather than a hand-rolled VM.
- **Data Locality** — [_text/16-data-locality.md](_text/16-data-locality.md) (no dedicated code example in this repo — text-only). Cache-friendly memory layout matters mainly at large scale/high entity counts; most mobile games hit their perf ceiling elsewhere (draw calls, GC, overdraw) well before data locality becomes the bottleneck. Relevant mainly to high-end mobile titles using Unity DOTS/ECS (see Section 5).

---

## Section 5 — Mobile-specific patterns beyond the book

*Game Programming Patterns* predates the current mobile F2P/live-ops era and doesn't cover UI-architecture or SDK-integration patterns that are now close to mandatory in shipped mobile games. Recommended additions for the thesis:

- **MVC / MVVM / MVP** — UI-heavy mobile games (shop screens, inventory, daily rewards, battle-pass) commonly separate presentation from state using one of these; MVVM with data-binding is increasingly common with Unity UI Toolkit. Builds directly on Observer (Tier 1).
- **Dependency Injection** (Zenject/Extenject, VContainer, or manual constructor injection) — the production alternative to Singleton/Service Locator that this repo's own Singleton text already gestures at; worth treating as the "next step" for a thesis chapter that critiques Singleton overuse.
- **Adapter** — used constantly to wrap platform SDKs with incompatible interfaces (Google Play Billing vs. Apple StoreKit, GameCenter vs. Google Play Games) behind one shared interface; closely related to Facade (Tier 2) and explicitly flagged as related in [_text/22-facade.md](_text/22-facade.md).
- **Repository / Remote Config pattern** — abstracting local save data vs. cloud save vs. server-driven remote config (A/B tests, live-ops event data) behind one data-access interface.
- **Strategy** — swappable algorithms/behaviors (AI difficulty tiers, monetization/ad-frequency strategies, matchmaking strategies); explicitly named as a close relative of State in [_text/6-state.md](_text/6-state.md).
- **Builder** — used for procedural level/character/loadout construction where a Factory's single-call construction isn't expressive enough.
- **Entity Component System (ECS) / Data-Oriented Design** — Unity DOTS/ECS is the mobile-scale evolution of the classic Component pattern (Tier 1) for games needing thousands of simulated entities under mobile CPU/battery budgets; worth pairing with the Data Locality discussion (Tier 3).

---

## Suggested empirical angle for the thesis

Since this is an academic study rather than a pure literature review, consider pairing the classification above with small reproducible measurements using this repo's existing dual implementations:

1. **Object Pool vs. naive Instantiate/Destroy** — [Assets/Patterns/18. Object Pool/Gun/Object pools/Simple](Assets/Patterns/18.%20Object%20Pool/Gun/Object%20pools/Simple) vs. plain `Instantiate`/`Destroy` bullets; measure frame time and `GC.Alloc` per frame on an actual Android/iOS device via the Unity Profiler. This directly substantiates the Tier 1 ranking of Object Pool.
2. **Simple list-search pool vs. optimized linked-list pool** — [.../Simple](Assets/Patterns/18.%20Object%20Pool/Gun/Object%20pools/Simple) vs. [.../Optimized](Assets/Patterns/18.%20Object%20Pool/Gun/Object%20pools/Optimized) vs. [.../UnityNative](Assets/Patterns/18.%20Object%20Pool/Gun/Object%20pools/UnityNative) — a clean "implementation quality within the same pattern" comparison.
3. **Singleton MonoBehaviour lifecycle bug** — reproduce the `OnDestroy`-during-quit teardown issue called out in [_text/5-singleton.md](_text/5-singleton.md) using [Assets/Patterns/5. Singleton/Scripts/SingletonUnity.cs](Assets/Patterns/5.%20Singleton/Scripts/SingletonUnity.cs) as a concrete failure-mode case study.

---

## Primary and secondary sources (from this repo's README)

- Robert Nystrom, *Game Programming Patterns* — [gameprogrammingpatterns.com](http://gameprogrammingpatterns.com) (primary source for all 19 "book" patterns).
- *Game Development Patterns with Unity 2021* (Packt).
- *Head First Design Patterns* (O'Reilly) — GoF-style coverage of Decorator, Factory, Facade, Template.
- *Game Programming Gems* / *Game Programming Gems 2*.
- [Refactoring Guru](https://refactoring.guru/design-patterns) — general GoF pattern reference.
- Unity, ["Level up your code with game programming patterns"](https://resources.unity.com/games/level-up-your-code-with-game-programming-patterns).

**Still to source for the mobile-specific angle** (not in the original README, needed to support Section 5 and the mobile-relevance claims): Unity DOTS/ECS documentation, Zenject/VContainer documentation, published GDC talks on mobile F2P architecture, and any academic papers specifically on mobile game software architecture (worth a literature search — flag to advisor if none are readily found, since most pattern literature is engine-agnostic or console/PC-focused).
