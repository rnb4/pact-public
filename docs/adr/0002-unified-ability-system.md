# Unified Ability System for All Unit Behaviors

Hero skills, item powers, basic attacks, monster abilities, and tower shots are all represented as `AbilityData` resources and executed through the same pipeline. There is no separate "attack system," "item effect system," or "monster skill system." This means designers can compose any unit behavior — including basic attacks and item effects — from the same building blocks (`Behavior`, `Trigger`, `Effect`) without programmer involvement for combinations of existing parts.

## Considered Options

- **Separate systems per context**: a dedicated attack loop, a separate item effect system, and ability-specific logic. Easier to prototype each in isolation but creates duplicated concepts (damage calculation, status application, projectile spawning) across systems that diverge over time.

## Extension: Unified Behavior Pipeline Across Abilities and Statuses

The same `Behavior = Trigger + Effect` abstraction used inside `AbilityRank` is also used inside `StatusData`. There is no separate `StatusBehavior` or `StatusTrigger` type hierarchy — all five trigger variants (`ActiveTrigger`, `EventTrigger`, `ContinuousTrigger`, `ConditionTrigger`, `QueryTrigger`) and all `Effect` subclasses are shared between ability behaviors and status behaviors.

This means a status can react to game events, fire on conditions, tick continuously, or contribute conditional modifications on demand — using the exact same building blocks as abilities. Statuses that respond to ability activations (`ActiveTrigger` conceptually firing on key press) and abilities that are queried for conditional modifiers (`QueryTrigger`) become natural expressions of the same system.

The alternative — separate `StatusTrigger` and `StatusEffect` hierarchies — would duplicate every primitive subclass and diverge over time, exactly as separate per-unit-type ability systems would have.
