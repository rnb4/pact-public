# Development Plan

---

## Phase 1 — Simulation Foundation

Core types everything else references. No logic yet — just data shapes.

### 1.1 Identity
- [x] `EntityId` — generational struct (`uint Index`, `uint Generation`) in `Shared/Snapshot/`
- [x] `Team` — enum (`Blue`, `Red`, `Neutral`) in `Shared/Snapshot/`
- [x] `FacingDirection` — `enum : byte` (N/NE/E/SE/S/SW/W/NW) in `Shared/Snapshot/`
- [x] `DamageType` — enum (`Physical`, `Magical`, `True`) in `Gameplay/Damage/`

### 1.2 Stats
- [x] `Source` struct — (`Caster Unit`, `Causer Resource`, `Cause IEffect`) in `Gameplay/Stats/`
- [x] `StatModifier` struct — (type, value, `Source`) in `Gameplay/Stats/`
- [x] `Stat` — runtime object, base + modifier list, computed `FinalValue` per formula, change signal; in `Gameplay/Stats/`
- [x] `ResourcePool` — (type, max, regen, current) runtime object; flesh out existing `ResourceType` enum; in `Gameplay/Stats/`

### 1.3 Content Resources (Godot Resource subclasses — C# class definitions only)
- [x] `HeroData` — base stats, per-level growth, resource pools, ability list; in `Gameplay/Entities/`
- [x] `MinionData` — base stats, pools, BasicAttack ref, gold/XP rewards; in `Gameplay/Entities/`
- [?] `MonsterData` — base stats, pools, ability list, BasicAttack, leash range, gold/XP; in `Gameplay/Entities/`
- [x] `AbilityRank` — behaviors list, cooldown, `ResourceCost?`, `BaseCastRange`, `MinHeroLevel`; in `Gameplay/Abilities/`
- [x] `AbilityData` — ordered `AbilityRank[]`, `GatesOnCooldown` bool; in `Gameplay/Abilities/`
- [x] `Behavior` — (`Trigger`, `Effect`) pair resource; in `Gameplay/Abilities/`
- [x] `StatusData` — `BaseDuration?`, stat modifiers, cost modifiers, cost conversions, behaviors, `ImpairmentType?`, stacking config; in `Gameplay/Status/`
- [x] `ItemData` — gold cost, `AbilityData` ref; in `Gameplay/Economy/`
- [?] `SpawnedSceneData` — `PackedScene` ref, resource ID string; in `Gameplay/Spawned/`
- [x] `VisualEffectData` — `PackedScene`, `AudioStream?`, playback params; in `Gameplay/Vfx/`

### 1.4 Unit & Component Base
- [?] `Component` — abstract `Node` base; in `Gameplay/Entities/`
- [x] `Unit` — flesh out: `CharacterBody3D`, holds component refs, exposes unit events (`OnHit`, `OnDamaged`, `OnHealed`, `OnKill`, `OnDeath`, `OnStatusApplied`, `OnLevelUp`), `EntityId`, `Team`, stat dictionary, resource pool dictionary; in `Gameplay/Entities/`

### 1.5 Status Supporting Types
- [x] `StackExpiry` enum (`LoseAll`, `CountDown`, `IndependentLifetimes`); in `Gameplay/Status/`
- [x] `StackApplication` enum (`AddStack`, `Refresh`); in `Gameplay/Status/`
- [x] `ImpairmentType` enum; in `Gameplay/Status/`
- [?] `StatusHandle` — opaque ref, carries `Source`; in `Gameplay/Status/`
- [x] `StatusStack` — per-stack source + duration tracking; in `Gameplay/Status/`
- [x] `StatusContext` struct — `StatusData`, `Source`, `FinalDuration`; in `Gameplay/Status/`

### 1.6 Damage Types
- [x] `DamageInstance` struct — amount, `DamageType`, `Source`, target `Unit`; in `Gameplay/Damage/`

### 1.7 Resource Cost Types
- [x] `ResourceCost` — interface with `Resolve(unit, statuses)` → `(ResourceType, float)[]`; in `Gameplay/Abilities/`
- [?] `SimpleResourceCost` — one type + amount; in `Gameplay/Abilities/`
- [x] `CompositeResourceCost` — list of child costs; in `Gameplay/Abilities/`
- [?] `ResourceCostModifier` — record on `StatusData` (global amount modifier); in `Gameplay/Status/`
- [x] `ResourceCostConversion` — record on `StatusData` (`FromType`, `ToType`, `Ratio`); in `Gameplay/Status/`

---

## Phase 2 — Ability System

### 2.1 EffectContext
- [x] Flesh out `EffectContext` — fields: `Caster Unit`, `Direction Vector3`, `Target` struct (`Position`, `Unit?`), `Source`, `AbilityData?`, `DamageInstance?`; in `Gameplay/Effects/`
- [x] `Target` struct — `Position` + `Unit?`; in `Gameplay/Effects/`

### 2.2 Triggers (Godot Resource subclasses)
- [ ] `ActiveTrigger` — `RequiresUnit bool`, if true checks if `Unit` in `Target` as `QueryTrigger`; in `Gameplay/Triggers/`
- [ ] `EventTrigger` — event name, subscription management; in `Gameplay/Triggers/`
- [ ] `ContinuousTrigger` — `TickInterval`; in `Gameplay/Triggers/`
- [ ] `ConditionTrigger` — `Predicate` ref; in `Gameplay/Triggers/`
- [ ] `QueryTrigger` — `Predicate` ref, fires on external query; in `Gameplay/Triggers/`

### 2.3 Predicates (Godot Resource subclasses)
- [ ] `HealthBelowThreshold`, `HasStatus`, `IsInRange`, `TargetHasStatus`; in `Gameplay/Predicates/`
- [ ] `AndPredicate`, `OrPredicate`, `NotPredicate`; in `Gameplay/Predicates/`

### 2.4 Effects (Godot Resource subclasses)
- [ ] `DamageEffect` — constructs `DamageInstance`, calls `DamageCalculator`; in `Gameplay/Effects/`
- [ ] `ApplyStatusEffect` — constructs `StatusContext`, calls `StatusManager.Apply()`; in `Gameplay/Effects/`
- [ ] `RemoveStatusEffect` — calls `StatusManager.Remove()` by source; in `Gameplay/Effects/`
- [ ] `SpawnSceneEffect` — calls `SpawnManager`; in `Gameplay/Effects/`
- [ ] `CompositeEffect` — ordered child effect list; in `Gameplay/Effects/`
- [ ] `ConditionalEffect` — wraps effect + predicate; in `Gameplay/Effects/`
- [ ] `DamageModifierEffect` — type filter, direction, pipeline step, modifier type + value; in `Gameplay/Effects/`

### 2.5 Ability Components
- [ ] `AbilitySet` component — flat `AbilityData[]`, manages `EventTrigger`/`ConditionTrigger` subscription lifecycle; in `Gameplay/Components/`
- [ ] `AbilityHandler` component — per-ability rank + cooldown tracking, gates execution, deducts resources; in `Gameplay/Components/`
- [ ] `ResourceCostCalculator` — static class, applies modifiers + conversions; in `Gameplay/Components/`

---

## Phase 3 — Server Systems

### 3.1 Game State
- [ ] Flesh out `GameState` — entity registry (`Dictionary<EntityId, Unit>`), per-team inhibitor destroyed flags, `GameTimer` tick counter, `GameEventQueue`; in `Shared/Snapshot/`
- [ ] `GameEventQueue` — accumulates `IServerEvent`s per tick; in `Server/Systems/`
- [ ] `ServerEventBus` — server singleton, typed C# events (`FirstBlood`, `TowerDestroyed`, etc.); in `Server/Systems/`

### 3.2 Snapshot & Network Types
- [ ] `GameStateSnapshot` — MemoryPack struct: `TickNumber uint`, fog-filtered entity registry entries (config + live state), global state; in `Shared/Snapshot/`
- [ ] `ServerEvents` — MemoryPack `IServerEvent[]` wrapper; in `Shared/Snapshot/`
- [ ] `RejectionReason` — typed enum or union for command rejection; in `Shared/Commands/`

### 3.3 Status System
- [ ] `StatusManager` — server system: `Apply()`, `Remove()`, owns timers, manages trigger subscriptions; in `Server/Systems/`

### 3.4 Damage
- [ ] `DamageCalculator` — static class: pre-mitigation query pass → mitigation → post-mitigation query pass; in `Server/Systems/`

### 3.5 Spawn System
- [ ] `SpawnManager` — server singleton, assigns `EntityId`s, adds to `GameState` registry; in `Server/Systems/`

### 3.6 Command Pipeline
- [ ] `CommandDispatcher` — deserializes `IClientCommand[]`, routes to handlers; in `Server/Handlers/`
- [ ] `PendingCast` struct — held per hero, approach-until-in-range logic; in `Server/`
- [ ] `MoveCommandHandler` — clears pending cast + attack target, sets nav destination; in `Server/Handlers/`
- [ ] `CastCommandHandler` — resolves to `PendingCast`; in `Server/Handlers/`
- [ ] `StopCommandHandler` — clears pending cast + attack target; in `Server/Handlers/`
- [ ] `CursorUpdateCommandHandler` — updates hero cursor position; in `Server/Handlers/`
- [ ] `BuyItemCommandHandler` — zone + gold + inventory validation, add ability; in `Server/Handlers/`
- [ ] `SellItemCommandHandler` — remove ability, refund gold; in `Server/Handlers/`
- [ ] `UpgradeAbilityCommandHandler` — point + rank + level validation, advance rank; in `Server/Handlers/`

---

## Phase 4 — Combat & Economy

- [ ] `AttackComponent` — Hero-only: holds attack target, recasts `BasicAttack` on cooldown; in `Gameplay/Components/`
- [ ] `DamageParticipation` — `HashSet<EntityId>` per unit, appended by `DamageCalculator`, read on death; in `Gameplay/Entities/`
- [ ] Gold tracking per hero in `GameState`; death distribution to participants + last-hit bonus
- [ ] Passive gold trickle (whole-number every N ticks)
- [ ] XP award on unit death (range-based, last-hit bonus)
- [ ] Level-up logic — threshold crossing, stat growth application, ability upgrade point grant
- [ ] `ShopZone` — `Area3D` in base, server re-validates on every buy command

---

## Phase 5 — AI

- [ ] `BehaviorTree` — composable C# class hierarchy (not visual plugin); in `Server/AI/`
- [ ] `MinionAI` component — lane march, basic attack nearest enemy, prioritize aggressors; in `Server/AI/`
- [ ] `MonsterAI` component — patrol, aggro, ability use, leash + health reset; in `Server/AI/`
- [ ] `TowerAI` component — target priority (heroes > retaliating minions > any minion), attack on tick; no BehaviorTree; in `Server/AI/`

---

## Phase 6 — Map & Game Flow

- [ ] `WaveSpawner` — fixed interval, per-lane per-team waves, super-minion tracking after inhibitor destroy; in `Server/Systems/`
- [ ] `Camp` — tracks live monster count, leash origin; starts `CampRespawnTimer` on full clear; in `Server/Systems/`
- [ ] `RespawnTimer` — per-hero, level-scaled duration, repositions + restores health/resources; in `Server/Systems/`
- [ ] `CampRespawnTimer` — on expiry calls `SpawnManager` for all camp monsters with new `EntityId`s; in `Server/Systems/`
- [ ] `InhibitorRespawnTimer` — restores `IsDead = false` + full health, notifies `NexusProtectionSystem` + `WaveSpawner`; in `Server/Systems/`
- [ ] `NexusProtectionSystem` — tracks destroyed inhibitor count per team, removes/reapplies `Undamageable`; in `Server/Systems/`
- [ ] `WinCondition` — subscribed to `NexusDestroyed`, emits `GameEndedEvent`; in `Server/Systems/`
- [ ] `FogOfWar` — per-team visible unit set, recomputed per tick from `VisionRadius` stats + vision-granting spawned scenes; in `Server/Systems/`

---

## Phase 7 — Spawned Scenes

- [ ] `SpawnedScene` — server-side node, holds spawning `EffectContext`, has `EntityId`; in `Gameplay/Spawned/`
- [ ] `MotionComponent` — abstract base; `LinearMotionComponent` (direction, speed, max range); `HomingMotionComponent` (tracks `Target.Unit`); in `Gameplay/Spawned/`
- [ ] `HitboxComponent` — `Area3D`, `OnEnterEffect?`, `OnExitEffect?`, `OnTickEffect?` + `TickInterval`; in `Gameplay/Spawned/`
- [ ] Scene templates in `scenes/entities/spawned/`: projectile, aura, AoE, summon

---

## Phase 8 — Network Layer

- [ ] `NetworkManager` AutoLoad — owns RPCs, peer lifecycle; in `src/Autoload/`
- [ ] Implement all 4 RPCs: `SubmitCommands`, `ReceiveGameStateSnapshot`, `ReceiveServerEvents`, `ReceiveCommandRejection`
- [ ] Tick queue — collect per-player commands, process at tick boundary
- [ ] Snapshot serialization with fog-of-war filtering per recipient
- [ ] `ServerEvents` fog-of-war filtering (positional events filtered, global events broadcast)

---

## Phase 9 — Client Systems

- [ ] `EntityRegistryReactor` — diffs snapshot registry, spawns/despawns `EntityView` nodes, applies live state; in `Client/Systems/`
- [ ] `EntityView` — lerps position, maps `FacingDirection` + camera yaw to sprite sheet row, plays animations from events; in `Client/Views/`
- [ ] `InputManager` — maps mouse/keyboard to `IClientCommand`s, sends `CursorUpdateCommand` every tick; in `Client/Systems/`
- [ ] `ClientGameEventBus` — routes `IServerEvent`s from `ReceiveServerEvents` to subscribers by type; in `Client/Systems/`
- [ ] `EffectSpawner` — maps `effectId` → `VisualEffectData`, instantiates client-side; in `Client/Systems/`

---

## Phase 10 — UI

- [ ] HUD — health/resource bars, ability slots + cooldown overlays, level/XP bar, gold display, kill feed; in `scenes/ui/hud.tscn` + `Client/UI/`
- [ ] Shop UI — item grid, buy/sell buttons (enabled only in ShopZone), gold display; in `scenes/ui/shop.tscn` + `Client/UI/`

---

## Phase 11 — Content

- [ ] Map scene — 3 lanes with waypoints, jungle camps, pre-placed structures (towers × 6 per team, inhibitors × 3 per team, nexus × 2), shop zones; in `scenes/map/`
- [ ] `Undamageable.tres` — StatusData with always-true QueryTrigger + DamageModifierEffect (PercentMultiply 0)
- [ ] First hero — `HeroData.tres`, `hero.tscn`, 4 `AbilityData.tres` + ranks
- [ ] Minions — `lane_minion.tres`, `super_minion.tres` MinionData resources
- [ ] Jungle monsters — at minimum 2 camp types with `MonsterData.tres` resources
- [ ] Tower, Inhibitor, Nexus structure scenes with stats + attack `AbilityData`
- [ ] Starter item set — 6–10 `ItemData.tres` resources
