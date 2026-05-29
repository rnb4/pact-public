# MOBA — Domain Glossary

## Entities

**Unit** — the base container node (`CharacterBody3D`) for any game entity. Has no behavior of its own. Behavior is entirely composed from child components. All entities (Heroes, Minions, Monsters, Structures) are Units.

**Hero** — a player-controlled Unit. Distinguished by an `AbilitySet` with unique abilities defined by a `HeroData` resource and a peer ID binding it to a connected client.

**Minion** — an AI-controlled Unit that spawns in waves and pushes lanes. Distinguished by a lane AI component. Defined by a `MinionData` resource.

**Monster** — an AI-controlled Unit that inhabits the jungle. More complex behavior than Minions. Defined by a `MonsterData` resource.

**Structure** — a Unit with no movement. Examples: towers, inhibitors, nexus. Has health and may deal damage. Defined as Godot scenes authored before runtime — stats, abilities, and components are configured in the scene directly. Pre-placed structures (towers, inhibitors, nexus) are embedded in the map scene and registered at match start with fixed `EntityId`s. Structures that spawn at runtime do so via `SpawnedSceneData` through `SpawnManager`.

**Team** — enum with three values: `Blue`, `Red`, `Neutral`. Assigned to every Unit at spawn. Used by fog-of-war filtering, damage target validation, AI targeting, and `GameStateSnapshot` entries.

**Component** — a child node attached to a Unit that provides a discrete behavior or data concern (movement, health, stats, abilities, AI, etc.). New entity types are created by assembling components, not by subclassing.

**AbilitySet** — a Component holding a flat list of `AbilityData` references representing all of a Unit's active abilities. Manages trigger subscription lifecycle — activates `EventTrigger`s and `ConditionTrigger`s on add, deactivates on remove. `ContinuousTrigger`s and `QueryTrigger`s require no subscription. Ability slot binding (Q/W/E/R) is a client-side input concern, not modeled here.

**AbilityHandler** — a Component that sits alongside `AbilitySet` and owns all runtime ability state. Tracks current rank and cooldown remaining per `AbilityData`. Locked abilities (rank not yet allocated) cannot fire any trigger. Gates `ActiveTrigger` execution on cooldown and resource sufficiency always. Gates `EventTrigger`, `ConditionTrigger`, and `ContinuousTrigger` execution on cooldown only when `AbilityData.GatesOnCooldown` is true. Resource sufficiency is checked for all trigger types when the current `AbilityRank` has a `ResourceCost`.

## Content

**HeroData** — a Godot `Resource` (`.tres`) defining a Hero's identity: base stats, a flat per-level growth value per stat (only stats with a non-zero growth entry scale with level), `ResourcePool` list (type, base max, regen rate per pool), sprite references, and ability list. No logic lives here.

**MinionData** — a Godot `Resource` (`.tres`) defining a Minion type: base stats, `ResourcePool` list, sprite references, `BasicAttack`, gold reward on death, and XP reward on death. No logic lives here.

**MonsterData** — a Godot `Resource` (`.tres`) defining a Monster type: base stats, `ResourcePool` list, sprite references, ability list, `BasicAttack`, leash range, gold reward on death, and XP reward on death. No logic lives here.

**AbilityData** — a Godot `Resource` (`.tres`) defining an ability as an ordered list of `AbilityRank`s. Used universally across all Unit types — hero skills, item powers, monster abilities, tower shots, and basic attacks are all `AbilityData`. Non-upgradeable abilities (items, monster abilities, basic attacks) have exactly one rank.

**AbilityRank** — a Godot `Resource` (`.tres`) defining one rank of an `AbilityData`: a list of `Behavior`s, a cooldown, an optional `ResourceCost`, a `BaseCastRange`, and a `MinHeroLevel` (minimum Hero level required to upgrade to this rank). Rank index 0 is the first unlock.

**GatesOnCooldown** — a `bool` on `AbilityData`. When true, `AbilityHandler` checks cooldown before firing any trigger type on this ability, including `EventTrigger`, `ConditionTrigger`, and `ContinuousTrigger`. `ActiveTrigger` always gates on cooldown regardless of this flag.

**ResourceCost** — a Godot `Resource` subclass held by `AbilityRank` defining what resources an ability consumes. Base class; `ResourceCostCalculator` calls `Resolve(unit, statuses)` to get final `(ResourceType, float)` pairs after applying modifiers and conversions.

**SimpleResourceCost** — a `ResourceCost` with one `ResourceType` and a base `Amount`.

**CompositeResourceCost** — a `ResourceCost` holding a list of child `ResourceCost`s. Resolves each child and returns all pairs combined. Used when an ability consumes multiple resource types simultaneously.

**Behavior** — a `Resource` pairing one `Trigger` with one `Effect`. The "when → what" unit of game logic. Used by both `AbilityRank` (ability behaviors) and `StatusData` (status behaviors). Multiple behaviors on the same owner are independent — different triggers fire different effects.

**Trigger** — a `Resource` subclass defining when a `Behavior` fires. Every trigger constructs an `EffectContext` before passing to its `Effect`. Usable on both `AbilityRank` and `StatusData` behaviors. Variants:
- `ActiveTrigger` — fires on player key press (conceptually an `EventTrigger` for input events). `Target.Position` = cursor world ray hit. `Target.Unit` = resolved unit if `RequiresUnit` is true, null otherwise.
- `EventTrigger` — named game event (OnHit, OnDamaged, OnKill, OnLevelUp, etc.) on a Unit or `ServerEventBus`. `Target` is populated from the event payload. Each event definition specifies what it provides.
- `ContinuousTrigger` — fires every server tick or at a configured `TickInterval`. `Target = { Position = owner.Position, Unit = owner }`.
- `ConditionTrigger` — holds a `Predicate` evaluated each server tick. Fires once when predicate transitions false→true. `Target = { Position = owner.Position, Unit = owner }`. To react to the reverse transition, add a second `Behavior` with a `NotPredicate` wrapping the same predicate.
- `QueryTrigger` — holds a `Predicate`. Fires when an external caller queries the behavior with an `EffectContext` and the predicate returns true. Used by `DamageCalculator` and other systems to collect conditional modifications on demand.

**EventTrigger subscription** — by default, an `EventTrigger` subscribes to the owning Unit's events when the ability is added to `AbilitySet`. For cross-unit subscriptions (e.g. "trigger when a marked enemy dies"), a paired `ActiveTrigger` behavior captures the target Unit via its `Effect` and registers the `EventTrigger` on that unit instead. The active cast establishes the binding.

**Effect** — a `Resource` subclass implementing one discrete outcome via `Execute(EffectContext)`. All world-affecting outcomes are `Effect` subclasses. New behavior requires a new subclass; reusing existing behavior requires only a new `.tres`.

**CompositeEffect** — an `Effect` that executes a list of child `Effect`s in sequence. Used when one trigger should produce multiple outcomes.

**ConditionalEffect** — an `Effect` wrapping another `Effect` and a `Predicate`. Executes the inner effect only if the predicate passes. Usable anywhere an `Effect` is expected, including inside `CompositeEffect`.

**Predicate** — a Godot `Resource` subclass with `Evaluate(EffectContext) → bool`. Used by `ConditionTrigger`, `QueryTrigger`, and `ConditionalEffect`. Primitive subclasses are defined in code (`HealthBelowThreshold`, `HasStatus`, `IsInRange`, `TargetHasStatus`, etc.). `AndPredicate`, `OrPredicate`, and `NotPredicate` are composites holding child `Predicate` lists, allowing designers to author complex conditions in the Godot inspector without writing code.

**DamageEffect** — an `Effect` that constructs a `DamageInstance` and passes it to `DamageCalculator`.

**ApplyStatusEffect** — an `Effect` that constructs a `StatusContext` from its referenced `StatusData`, `context.Source`, and an optional duration, then calls `StatusManager.Apply()`.

**RemoveStatusEffect** — an `Effect` that calls `StatusManager.Remove()` for all active statuses on the target Unit whose `Source` matches `context.Source`. Used as the false-transition leg of a `ConditionTrigger` pair to undo what `ApplyStatusEffect` applied.

**SpawnSceneEffect** — an `Effect` that calls `SpawnManager` to instantiate a `SpawnedScene` from a referenced `SpawnedSceneData` at the cast position. Passes the current `EffectContext` to the spawned scene. Motion behavior is defined by whichever `MotionComponent` the scene contains, initialized from `EffectContext` at spawn.

**Target** — a struct carried by `EffectContext`. Contains `Vector3 Position` (always present — a world position) and `Unit? Unit` (present only when a unit was resolved; null for ground-targeted and self-targeted contexts).

**EffectContext** — a struct passed to `Effect.Execute()` and `Predicate.Evaluate()`. Fields: caster `Unit` (always present), direction vector (caster → `Target.Position`, always present), `Target` (always present), `Source` (always present), `AbilityData?` (present for ability-triggered effects, absent for status-triggered effects), `DamageInstance?` (present only during damage calculation). Effects and predicates null-check optional fields as needed.

**RequiresUnit** — a `bool` on `ActiveTrigger`. Client input concern only — drives cursor indicator and server validation. `false`: any world position valid, `Target.Unit` is null. `true`: cursor must resolve a valid Unit, which populates `Target.Unit`.

## Spawning

**SpawnManager** — server-side singleton responsible for all game-state entity spawning. Adds new entities to `GameState`'s entity registry. `EntityRegistryReactor` on each client diffs the registry each tick and spawns or despawns local scene nodes accordingly. All server-side spawning (`WaveSpawner`, `SpawnSceneEffect`, `CampRespawnTimer`, etc.) routes through here.

**IGameEvent** — MemoryPack union of pure data records representing all one-shot server → client notifications not derivable from state sync. Two categories: positional events (fog of war filtered) and global events (sent to all clients). Mirrors `IClientCommand` in structure. Variants:
- `VfxEvent(string effectId, Vector3 position)` — fog of war filtered by position
- `SoundEvent(string effectId, Vector3 position)` — fog of war filtered by position
- `FloatingTextEvent(string text, Vector3 position)` — fog of war filtered by position
- `AttackEvent(EntityId attacker)` — fog of war filtered by entity visibility. `EntityView` plays the unit's attack animation.
- `CastEvent(EntityId caster, int abilityIndex)` — fog of war filtered by entity visibility. `EntityView` plays the animation for the ability at that slot in the unit's `AbilitySet`.
- `AnnouncementEvent(string effectId)` — global
- `GameEndedEvent(int winningTeam)` — global

**GameEventQueue** — server-side accumulator filled during tick execution. At tick boundary, pending `IGameEvent`s are serialized into `ServerEvents` and cleared. Fog of war filtering applied to positional events; global events sent to all clients unconditionally.

**VisualEffectData** — a Godot `Resource` (`.tres`) bundling the assets needed to play one visual or audio effect: a `PackedScene` (particles, mesh, etc.), an optional `AudioStream`, and playback parameters. Identified by an `effectId` string used in `IGameEvent`s. `EffectSpawner` resolves `effectId` → `VisualEffectData` and instantiates it client-side.

**EffectSpawner** — client-side consumer of visual `IGameEvent`s received via `ReceiveServerEvents`. Maps `effectId` to `VisualEffectData` and instantiates it. Subscribed to `ClientGameEventBus` for visual event types only.

**ClientGameEventBus** — client-side dispatcher that receives `ServerEvents` from `ReceiveServerEvents` and routes each `IGameEvent` to subscribed handlers by type. `EffectSpawner` handles visual events; game systems handle gameplay events.

## Commands

**Command** — a pure MemoryPack-serializable data record implementing `IClientCommand`. Represents a single player intent: `MoveCommand`, `CastCommand`, `BuyItemCommand`, `SellItemCommand`, `StopCommand`, `CursorUpdateCommand`, `UpgradeAbilityCommand`. No logic — data only.

**StopCommand** — a `Command` that halts movement, clears the current `PendingCast`, and clears `AttackComponent`'s current target. No persistent state.

**MoveCommand** — a `Command` carrying a `Vector3` world position. Clears the current `PendingCast` and `AttackComponent` target, then sets the Hero's movement destination. Server computes a path via `NavigationServer3D` and advances the unit along it each tick.

**CursorUpdateCommand** — a `Command` sent every tick carrying `Vector3 CursorWorldPosition`. No rejection path. Handler updates the Hero's last known cursor position server-side, used to compute `FacingDirection` when idle.

**CastCommand** — a `Command` carrying raw client input: ability index (slot in `AbilitySet`), cursor world position, optional target `EntityId`. The server's `IClientCommandHandler<CastCommand>` resolves it into a `PendingCast`. Right-clicking an enemy unit issues a `CastCommand` for the `BasicAttack` ability.

**PendingCast** — server-side struct held per Hero representing an unresolved `CastCommand`. Each tick: compute effective cast range as `AbilityRank.BaseCastRange` modified by the caster's `CastRange` stat `FinalValue` (which incorporates all active status `StatModifier`s). If Euclidean distance from caster to `Target.Position` (or `Target.Unit.Position`) is within effective range, execute the ability (construct `EffectContext`, pass to `AbilityHandler`) and clear. If not, recalculate path to target via `NavigationServer3D` and advance the caster one tick of movement. Path recalculates every tick to track moving targets. Cleared by any new `Command` or if `Target.Unit` dies before execution.

**CommandDispatcher** — server-side router that deserializes an incoming `IClientCommand` and routes it to the registered `IClientCommandHandler<T>` for that command type.

**IClientCommandHandler<T>** — server-side handler for one `Command` type. Validates the command against `GameState` and either executes it or rejects it. Contains all logic for that command.

**Command Rejection** — when a handler rejects a command, the server fires `ReceiveCommandRejection` to the originating client with a typed `RejectionReason` for UI feedback (e.g. insufficient resources, invalid target, out of range).

**Tick Queue** — server collects all incoming `Command`s within a game tick into a per-player queue and processes them together at the tick boundary. Ensures no ping advantage on contested actions.

**Client Prediction** — none. All entities including the local Hero lerp toward their last known server position. No client-side movement simulation.

## Events

**Unit Events** — typed C# events fired directly on a Unit instance. `EventTrigger`s subscribe here. Each event defines the `Target` it provides to the resulting `EffectContext`:
- `OnHit(Source source, Unit attacker)` — fired on the unit that was hit. `Target.Unit` = attacker, `Target.Position` = attacker's position.
- `OnDamaged(Source source, float amount, DamageType type)` — `Target.Unit` = attacker (source caster), `Target.Position` = attacker's position.
- `OnHealed(Source source, float amount)` — `Target.Unit` = healer (source caster), `Target.Position` = healer's position.
- `OnKill(Unit killed)` — `Target.Unit` = killed unit, `Target.Position` = killed unit's position.
- `OnDeath(Source killer)` — `Target.Unit` = killer unit (null if environmental), `Target.Position` = killer's position.
- `OnStatusApplied(StatusHandle handle)` — `Target.Unit` = status source caster, `Target.Position` = caster's position.
- `OnLevelUp(int newLevel)` — no other unit. `Target = { Position = owner.Position, Unit = owner }`.

**ServerEventBus** — server-side singleton publishing game-wide events not owned by any single Unit. Server systems and `EventTrigger`s reacting to global game state subscribe here. Each event defines the `Target` it provides to the resulting `EffectContext`:
- `FirstBlood(Unit killer, Unit killed)` — `Target.Unit` = killer, `Target.Position` = killer's position.
- `TowerDestroyed(Unit tower, Source destroyer)` — `Target.Unit` = destroyer unit, `Target.Position` = tower's world position.
- `InhibitorDestroyed(Unit inhibitor, Source destroyer)` — `Target.Unit` = destroyer unit, `Target.Position` = inhibitor's world position.
- `NexusDestroyed(Unit nexus, Source destroyer)` — `Target.Unit` = destroyer unit, `Target.Position` = nexus's world position. Also triggers `WinCondition`.
- `WaveSpawned(int lane, Team team)` — no unit context. `Target = { Position = lane spawn origin, Unit = null }`.
- `GameEnded(Team winner)` — no unit or position context. `Target = { Position = Vector3.Zero, Unit = null }`.

## Damage

**DamageType** — enum with three values: `Physical` (mitigated by Armor), `Magical` (mitigated by MagicResist), `True` (no mitigation).

**DamageInstance** — a struct describing one damage event: amount, `DamageType`, `Source`, and target Unit.

**DamageCalculator** — server-side static class that takes a `DamageInstance` and returns final damage. Pipeline: (1) base damage from `DamageInstance.amount` as set by `DamageEffect`, (2) pre-mitigation modifications — queries all `QueryTrigger` `Behavior`s on source and target, collecting `DamageModifierEffect`s whose predicates pass, (3) mitigation from target stats, (4) post-mitigation modifications — same query pass. All damage flows through here. Adding a new `DamageType` requires a new mitigation stat on all Units, new base stat values in all content resources, and a new formula branch here.

**DamageModifierEffect** — an `Effect` subclass applied via `QueryTrigger` `Behavior`s on active statuses and abilities. Fields: `DamageType` filter (specific type or `All`), direction (`Outgoing` from source or `Incoming` to target), `PipelineStep` (`PreMitigation` or `PostMitigation`), modifier type (FlatAdd, PercentAdd, PercentMultiply), and value. `DamageCalculator` executes these during its query pass; the `EffectContext` passed includes the current `DamageInstance?`.

## Status System

**StatusManager** — server-side system that owns all active status state on all Units. Exposes `Apply(unit, StatusContext)` → `StatusHandle` and `Remove(handle)`. On apply: activates `EventTrigger`s and `ConditionTrigger`s from the status's `Behavior`s (`ContinuousTrigger`s and `QueryTrigger`s require no activation). On remove or expiry: deactivates those subscriptions. Owns timers for timed statuses. Sources are fire-and-forget after calling Apply.

**StatusContext** — a struct passed to `StatusManager.Apply()` containing: `StatusData`, `Source`, and resolved `FinalDuration`. Mirrors the role `EffectContext` plays for `Effect.Execute()`. `FinalDuration` is computed by `ApplyStatusEffect` from a base duration (from `StatusData.BaseDuration`, optionally overridden per effect), modified by caster stats (e.g. `StatusDurationBonus`) and target stats (e.g. `Tenacity`).

**StatusData** — a Godot `Resource` (`.tres`) defining a status effect's contents: a `BaseDuration` (optional default duration before stat modification), a list of `StatModifier`s, a list of `ResourceCostModifier`s, a list of `ResourceCostConversion`s, a list of `Behavior`s, an optional `ImpairmentType`, and stacking rules (`StackExpiry`, `StackApplication`, `MaxStacks`). `Behavior`s use any `Trigger` type — `ContinuousTrigger` for tick effects, `EventTrigger` for reactive effects, `ConditionTrigger` for conditional effects, `QueryTrigger` for on-demand modifier contribution. No removal logic lives here.

**ImpairmentType** — an enum value optionally set on `StatusData` identifying the movement or action restriction this status imposes (Stunned, Rooted, Silenced, etc.). Impairments are authored as dedicated `StatusData` resources (e.g. `Stunned.tres`) that appear as distinct entries on the status bar. Re-applying an impairment obeys the same stacking rules as any other status.

**StatusHandle** — an opaque reference returned by `StatusManager.Apply()`. Carries the `Source` that applied it. Held by any source that may need to trigger early removal (conditional statuses, auras, item unequip). Timed statuses are removed automatically; handle still usable for early removal.

**StatusStack** — when a `StatusData` supports stacking, each stack is tracked individually with its own `Source` and duration. Enables per-source stack queries (e.g. remove all stacks applied by a specific caster) and correct attribution of stack-based effects.

**StackExpiry** — enum on `StatusData` defining how stacks leave: `LoseAll` (all stacks share one timer, expire simultaneously), `CountDown` (stacks expire one at a time in order, each with the same duration), `IndependentLifetimes` (each stack carries the duration it was applied with and runs its own independent timer — unit remains affected until all stacks have individually expired).

**StackApplication** — enum on `StatusData` defining what a new application does: `AddStack` (adds a new stack up to `MaxStacks`), `Refresh` (resets the active duration without adding a stack — with `LoseAll` resets the shared timer, with `CountDown` resets the front-of-queue timer, with `IndependentLifetimes` is a no-op).

**ResourceCostModifier** — a record on `StatusData` read by `ResourceCostCalculator`. Applies a global amount modifier (FlatAdd, PercentAdd, PercentMultiply) to all resource costs on any ability cast by the affected Unit, regardless of `ResourceType`.

**ResourceCostConversion** — a record on `StatusData` read by `ResourceCostCalculator`. Fields: `FromType`, `ToType`, `Ratio` (float, default 1.0). When an ability would cost `FromType`, that cost is instead paid from `ToType` at `Amount × Ratio`. Applied after amount modifiers.

## Spawned Scenes

**SpawnedSceneData** — a Godot `Resource` (`.tres`) wrapping a `PackedScene` for use as a spawnable entity. Gives the scene a resource ID the `GameStateSnapshot` can carry. Referenced by `SpawnSceneEffect` instead of a raw `PackedScene`. `EntityRegistryReactor` instantiates the correct scene client-side by looking up this ID.

**SpawnedScene** — a server-side scene node instantiated by `SpawnSceneEffect`. Carries the `EffectContext` from the ability that spawned it. Has an `EntityId` and appears in `GameStateSnapshot` with its `SpawnedSceneData` resource ID as the config entry. Makes no assumptions about internal structure — may be a hitbox, a summoned Unit, a wall, a visual-only object, or anything else. Behavior is defined by the scene's own components.

**MotionComponent** — a Component subclass on a `SpawnedScene` that advances its position each server tick. Multiple subclasses define distinct movement behaviors — a `SpawnedScene` uses whichever fits its design. Examples: `LinearMotionComponent` (constant direction, speed, max range — destroys scene on range expiry), `HomingMotionComponent` (adjusts direction toward `Target.Unit` each tick). New movement patterns require a new subclass.

**HitboxComponent** — a Component on a `SpawnedScene` that uses `Area3D` to detect unit overlap. Exposes optional `OnEnterEffect`, `OnExitEffect`, and `OnTickEffect` (`Effect`s executed against overlapping Units). Scenes that need collision-based effects include this component; scenes that don't (walls, summoned Units, visual objects) omit it.

**OnEnterEffect** — `Effect` on `HitboxComponent` executed against each Unit entering the area. Uses inherited `EffectContext` with entering Unit as target.

**OnExitEffect** — `Effect` on `HitboxComponent` executed against each Unit exiting the area. Designer-defined — no automatic reversal.

**OnTickEffect** — `Effect` on `HitboxComponent` executed at `TickInterval` against all Units currently inside the area.

Common scene configurations:
- **Projectile** — `HitboxComponent` paired with a `MotionComponent` subclass. `OnEnterEffect` is the hit payload. Destroyed on first hit or when the `MotionComponent` reaches its termination condition (e.g. max range).
- **Aura** — `HitboxComponent` centered on and following a Unit. `OnEnterEffect` fires on entering Units; `OnExitEffect` fires on exiting Units. No automatic reversal — designer decides what each does.
- **AoE** — stationary `HitboxComponent`. Instant (`OnEnterEffect` against all overlapping at spawn) or persistent (`OnTickEffect`).
- **Summon** — a full `Unit` node spawned into the world with its own AI and ability components.

## Stats

**ResourceType** — enum identifying a resource pool (Mana, Stamina, Health, Stacks, etc.). Used by `ResourceCost` and `ResourceCostConversion`.

**ResourcePool** — runtime object on a Unit tracking one resource type. Max value and optional regeneration rate are set from the unit's data resource at spawn; current value changes during the match. `AbilityHandler` checks pool sufficiency via `ResourceCostCalculator` before executing any trigger with a `ResourceCost`; deducts on success. Units with no pool entry for a given `ResourceType` cannot pay costs of that type.

**Stat** — runtime object on a Unit tracking one numeric value (e.g. MaxHealth, Armor, AttackDamage, MoveSpeed). Base value is set from the unit's data resource at spawn. Holds a runtime list of `StatModifier`s added and removed during the match. Exposes a computed `FinalValue`. Fires a signal when `FinalValue` changes.

**StatModifier** — a struct describing one unconditional modification to a Stat. Has a type (FlatAdd, PercentAdd, PercentMultiply), a numeric value, and a `Source` used for clean removal.

**Source** — a struct identifying the origin of a `StatModifier` or status effect at three tiers: `Caster` (the Unit that initiated the effect), `Causer` (the `AbilityRank` or `StatusData` whose `Behavior` fired the effect), and `Cause` (the specific `Effect` that applied it). Enables removal at any granularity — all modifiers from a caster, all modifiers from a specific rank or status, or one specific modifier from one effect.

**FinalValue formula** — `(base + ΣFlatAdd) × (1 + ΣPercentAdd) × ΠPercentMultiply`. Applied in that order.

## Economy

**Gold** — per-Hero currency. Earned from kill participation (any Hero who dealt damage to the unit before it died), last-hit bonus, and passive trickle on a fixed tick interval (a whole number of gold every N ticks). Tracked server-side only.

**DamageParticipation** — a `HashSet<EntityId>` held per Unit tracking which Heroes have dealt damage to it. Appended to by `DamageCalculator` on each damage event. Read by death handling to distribute gold to all participants. Cleared on death.

**ShopZone** — a static `Area3D` in each team's base, defined in the map scene. Client checks the local Hero's position against it each frame to enable or disable shop purchase buttons. Server re-validates zone membership on every `BuyItemCommand` regardless of client state.

**Shop** — UI openable from anywhere. Purchases require the Hero to be inside a `ShopZone`; sells have no location requirement. Server validates zone membership, sufficient gold, item not already owned, and inventory not full (max 6 items) on `BuyItemCommand` receipt; rejects with a typed `RejectionReason` if any check fails. On successful purchase, adds the `ItemData`'s `AbilityData` to the Hero's `AbilitySet`. On successful sell, removes it and refunds a portion of gold.

## Progression

**Experience (XP)** — awarded server-side to all Heroes within a fixed global XP range when a Unit dies. Last-hitting Hero receives a bonus. Accumulates toward the next level threshold.

**Level** — a Hero's current power tier, increased when XP crosses a threshold. Grants base stat growth (flat per-level values defined in `HeroData`) and one ability upgrade point.

**AbilityUpgradePoint** — a per-Hero currency earned on level up. Spent via `UpgradeAbilityCommand` to advance one ability to its next rank. Server validates: point available, ability not at max rank, Hero level ≥ `AbilityRank.MinHeroLevel`.

**ItemData** — a Godot `Resource` (`.tres`) defining a purchasable item: gold cost and one `AbilityData`. Purchasing adds that `AbilityData` to the Hero's `AbilitySet`. Selling removes that `AbilityData` from `AbilitySet`; `AbilityHandler` then releases any `StatusHandle`s it was tracking for that ability.

## Visibility

**FogOfWar** — server-only visibility system. Maintains a per-team set of currently visible Units, recomputed each tick from each team's Units' `VisionRadius` stats and any vision-granting `SpawnedScene`s with their own `VisionRadius`. Clients receive state only for Units their team can see.

**VisionRadius** — a stat on a Unit defining how far it reveals the map for its team.

## Network

**Server** — the authoritative dedicated game server. All simulation runs here. Clients receive state, not simulation results.

**InputManager** — client-side system that reads mouse and keyboard input each frame and translates it into `IClientCommand`s submitted to the server each tick. Sends `CursorUpdateCommand` every tick. Handles ability slot mapping (Q/W/E/R → ability index in `AbilitySet`).

**Client** — a player's game instance. Sends `IClientCommand[]` to server each tick via `SubmitCommands`. Receives `GameStateSnapshot` and `ServerEvents` via two separate RPCs and reacts via `EntityRegistryReactor` and `ClientGameEventBus`.

**GameState** — server-side authoritative representation of all simulation state. Owns the entity registry, global state (per-team per-lane inhibitor destroyed flags, elapsed tick counter), and the current tick's pending `GameEventQueue`. Read by `IClientCommandHandler<T>`s during validation and execution. Serialized into `GameStateSnapshot` + `ServerEvents` at tick boundary for client transmission.

**GameTimer** — the elapsed tick counter in `GameState`, incremented each server tick. Converted to wall time by dividing by tick rate. Included in `GameStateSnapshot` for HUD display. Systems that gate on elapsed time (e.g. timed spawns) read it directly from `GameState`.

**EntityId** — a generational ID struct (`uint Index`, `uint Generation`) assigned to every entity at spawn. Generation increments when an entity is permanently removed from the registry and its index is recycled — preventing stale references to a new entity at the same index. Heroes are never removed from the registry through death and respawn; their `EntityId` remains constant for the duration of the match.

**FacingDirection** — `enum : byte` with 8 values (N, NE, E, SE, S, SW, W, NW). Carried per entity in `GameStateSnapshot`. Server computes it each tick: moving → nearest direction to velocity, casting → nearest direction to (Target.Position − caster), idle → nearest direction to (CursorWorldPosition − caster) from last `CursorUpdateCommand`. `EntityView` maps `FacingDirection` to a sprite sheet row on its `AnimatedSprite3D`, offset by camera yaw, to select the correct directional animation.

**GameStateSnapshot** — MemoryPack-serialized struct transmitted unreliably each tick. Contains `uint TickNumber` (clients discard packets where `TickNumber` ≤ last applied), the full entity registry for this client's visible set (fog of war filtered), and global state. The entity registry is authoritative for entity existence — an entity present in the registry exists; an entity absent is despawned. Each entry carries both config fields (EntityType, content resource ID, team, etc.) and live state (position, resource pool values, a list of active statuses as `(StatusData resource ID, stack count)` pairs, `FacingDirection`, `IsDead`). The HUD resolves status icons by looking up `StatusData` by resource ID. The content resource ID refers to the appropriate data resource for the entity type — `HeroData` for Heroes, `MinionData` for Minions, `MonsterData` for Monsters, `SpawnedSceneData` for runtime-spawned scenes and structures. Pre-placed structures carry no content resource ID — `EntityRegistryReactor` identifies them by their fixed `EntityId` assigned at match start. `EntityRegistryReactor` uses `EntityType` to interpret entries. Config fields are small enough to send every tick.

**ServerEvents** — MemoryPack-serialized `IGameEvent[]` transmitted reliably each tick via `ReceiveServerEvents`. Contains all `IGameEvent`s accumulated in `GameEventQueue` this tick, with fog of war filtering applied to positional events and global events broadcast to all clients.

**EntityView** — client-side scene node representing one entity. Spawned and despawned by `EntityRegistryReactor`. Receives live state from the snapshot each tick and determines the correct visual state: idle/walk from position delta, death from `IsDead` flag, attack/cast from `AttackEvent`/`CastEvent` received via `ClientGameEventBus`. Uses `FacingDirection` and camera yaw offset to select the correct sprite sheet row. Contains no simulation logic.

**EntityRegistryReactor** — client-side system that diffs the incoming `GameStateSnapshot` entity registry against its local registry. New `EntityId` in snapshot: instantiate the correct `EntityView` scene from `EntityType`, apply config and state. `EntityId` absent from snapshot: despawn the node. Existing entity: apply live state only. All entities including the local Hero lerp position toward last known server position each frame. Self-healing — a dropped snapshot causes at most one tick of stale state before the next snapshot corrects it.

**NetworkManager** — Godot AutoLoad node. Owns all transport RPCs and peer lifecycle (connect, disconnect, timeout). Game systems interact only with `CommandDispatcher` and `GameState` — never with `NetworkManager` directly. RPCs:
- `SubmitCommands(byte[] data)` — `RpcMode.AnyPeer`, reliable. Client sends MemoryPack-serialized `IClientCommand[]` for the tick.
- `ReceiveGameStateSnapshot(byte[] data)` — `RpcMode.Authority`, unreliable. Server sends fog-of-war-filtered `GameStateSnapshot` at tick boundary.
- `ReceiveServerEvents(byte[] data)` — `RpcMode.Authority`, reliable. Server sends `ServerEvents` at tick boundary. Reliable because `IGameEvent`s are one-shot — a dropped packet means permanent loss.
- `ReceiveCommandRejection(byte[] data)` — `RpcMode.Authority`, reliable. Server notifies originating client of a rejected command with a typed `RejectionReason`.

## Combat

**ResourceCostCalculator** — server-side static class called by `AbilityHandler` before executing any trigger when the current `AbilityRank` has a `ResourceCost`. Reads the `AbilityRank`'s `ResourceCost`, applies global `ResourceCostModifier`s from the caster's active statuses, applies `ResourceCostConversion`s, and returns final `(ResourceType, float)` pairs. `AbilityHandler` checks that all pools are sufficient; if so, deducts from each `ResourcePool` and proceeds. If not, rejects the trigger.

**BasicAttack** — an `AbilityData` resource assigned to a Unit defining what its basic attack does. Fully composable via `Effect`s — melee, ranged, on-hit effects, etc. Interchangeable with cast abilities in the effect pipeline.

**AttackComponent** — a Hero-only Component that holds the current attack target and recasts `BasicAttack` on cooldown against it. Target is set when a `BasicAttack` `CastCommand` resolves. Cleared by `StopCommand`, `MoveCommand`, any non-`BasicAttack` `CastCommand`, or when the target dies or leaves range. AI-controlled Units call `AbilityHandler` directly — they do not use `AttackComponent`.

## AI

**BehaviorTree** — composable C# class hierarchy used for AI decision-making. Runs server-side each tick on Units with an AI component. Not a visual editor plugin.

**MinionAI** — a Component running a `BehaviorTree` that implements lane-march behavior: advance down lane, call `AbilityHandler` to execute `BasicAttack` against nearest enemy in range, prioritize enemies targeting this Unit.

**MonsterAI** — a Component running a `BehaviorTree` that implements camp behavior: patrol, aggro on proximity, call `AbilityHandler` to execute abilities against target, leash back to camp origin if pulled beyond leash range, reset health and return on leash break.

**TowerAI** — a Component on tower Structures that selects a target and calls `AbilityHandler` to execute the tower's attack `AbilityData` each tick. Target priority: enemy Heroes in range first, then Minions that attacked a friendly Minion, then any enemy Minion in range. No `BehaviorTree` — selection logic is simple enough to implement directly.

## Map

**Pathfinding** — all unit movement (Heroes, Minions, Monsters, `PendingCast` approach) uses Godot's `NavigationServer3D` server-side. Range checks use Euclidean distance, not path distance. `MinionAI` and `MonsterAI` request paths to their next waypoint or target each tick.

**Lane** — a path between bases along which Minions march and Structures are placed. Represented as an ordered list of waypoint `Node3D` markers in the static map scene. `MinionAI` traverses waypoints in sequence. Each Lane receives its own independent Minion wave.

**WaveSpawner** — server-side system that fires at fixed intervals, spawning one Minion wave per Lane per team. Subscribed to `InhibitorDestroyed` and `InhibitorRespawnTimer` expiry to track which Lanes have active super-minion spawning. After an enemy Inhibitor is destroyed, spawns super-minions in addition to normal Minions on that Lane until the Inhibitor respawns.

**SuperMinion** — a distinct `MinionData` resource with higher base stats than normal Minions. `WaveSpawner` includes one per wave on Lanes where the enemy Inhibitor is destroyed. Uses the same `MinionAI` as normal Minions.

**RespawnTimer** — server-side timer started on Hero death. Duration scales with Hero level. On expiry, resets the Hero's position to their base spawn point and restores full health and resources directly in `GameState` — no `SpawnManager` involvement since the Hero never leaves the entity registry. Minions and Monsters do not use this — Minions respawn via `WaveSpawner`, Monsters via `CampRespawnTimer`.

**Jungle** — the area between Lanes where Monsters spawn.

**Camp** — a named group of Monster spawn points in the Jungle, treated as a unit for respawn and leash purposes. Tracks how many of its Monsters are currently alive. When all Monsters in a Camp die, starts a `CampRespawnTimer`. Provides the leash origin used by `MonsterAI`.

**CampRespawnTimer** — server-side timer started when a Camp is fully cleared. On expiry, calls `SpawnManager` to instantiate all Camp Monsters at their spawn points with fresh `EntityId`s. Monsters leave the entity registry on death and receive new `EntityId`s on respawn — unlike Heroes, their identity does not persist through death.

**Nexus** — the core Structure of each team's base. Destroying the enemy Nexus is the win condition. Starts the match with `Undamageable` applied; becomes attackable only when an enemy Inhibitor is destroyed.

**Undamageable** — a permanent `StatusData` with a `QueryTrigger` `Behavior` (always-true `Predicate`) and a `DamageModifierEffect` (Incoming, All, PostMitigation, PercentMultiply 0). Applied to the Nexus at match start. Removed by `NexusProtectionSystem` when any enemy Inhibitor is destroyed; re-applied when all destroyed Inhibitors have respawned.

**Inhibitor** — a Structure that, when destroyed, enables super-minions to spawn for the enemy team and exposes the Nexus to attack. Stays in the entity registry through destruction (`IsDead = true`); `InhibitorRespawnTimer` restores it rather than re-spawning it.

**InhibitorRespawnTimer** — server-side timer started when `InhibitorDestroyed` fires on `ServerEventBus`. Duration is a fixed designer-defined constant. On expiry, restores the Inhibitor to full health and clears its `IsDead` flag in `GameState`. Notifies `NexusProtectionSystem` and `WaveSpawner` to re-evaluate their state.

**NexusProtectionSystem** — server-side system subscribed to `InhibitorDestroyed` on `ServerEventBus` and to `InhibitorRespawnTimer` expiry. Tracks how many Inhibitors are destroyed per team. Removes `Undamageable` from the enemy Nexus when the first Inhibitor is destroyed; re-applies it when the last destroyed Inhibitor respawns.

**WinCondition** — server-side system subscribed to `NexusDestroyed` on `ServerEventBus`. On receipt, emits `GameEndedEvent` via `GameEventQueue`, transmitted to all clients as a global `IGameEvent` in the next `ServerEvents`.
