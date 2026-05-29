# QueryTrigger for Conditional Damage Modifications

Conditional damage modifications (e.g. "+20% damage against slowed targets," "reduce incoming physical damage by 15% while above 50% health") are authored as `Behavior`s with a `QueryTrigger` and a `DamageModifierEffect`. `DamageCalculator` queries all active `QueryTrigger` behaviors on the source and target units at each pipeline step, evaluates each predicate against the current `EffectContext` (which includes `DamageInstance?`), and applies the `DamageModifierEffect` if the predicate passes.

## Considered Options

- **Flat `DamageModifier` records on `StatusData`**: simple struct read directly by `DamageCalculator` — no predicate, no behavior, no context. Unconditional modifiers are trivial; conditional modifiers require a separate `ConditionalDamageModifier` type that duplicates predicate infrastructure already present in the trigger system.
- **Separate conditional modifier type**: a `ConditionalDamageModifier` pairing a `DamageModifier` with a `Predicate`. Avoids the full behavior abstraction but creates a parallel predicate pipeline — predicates would need separate evaluation context from the ability/status predicate system, leading to duplication.

## Why QueryTrigger

`QueryTrigger` reuses the existing `Predicate` system and `EffectContext` (including `DamageInstance?`) without any new infrastructure. A modifier that applies only against specific damage types, directions, or pipeline steps is expressed as a `Predicate` + `DamageModifierEffect` — the same authoring pattern designers use everywhere else. Adding a new conditional modifier requires only a new `.tres`, not a new type.

## Consequences

- `DamageCalculator` must iterate all active `QueryTrigger` behaviors on both units each damage event. Acceptable at MOBA entity counts.
- Unconditional damage modifiers are also expressed as `QueryTrigger` behaviors (always-true predicate). Slightly more overhead than a flat record; acceptable given the eliminated duplication.
