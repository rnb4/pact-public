# Structures Defined as Scenes, Not Data Resources

Structures (towers, inhibitors, nexus, and any runtime-spawned structures) are authored as Godot scenes with stats, abilities, and components configured directly in the scene. There is no `StructureData` resource analogous to `HeroData`, `MinionData`, or `MonsterData`.

## Considered Options

- **`StructureData` resource**: consistent with other entity types. Gives structures a content resource ID the `GameStateSnapshot` can carry, enables data-driven stat inspection, and makes structures configurable without touching scenes. Requires `EntityRegistryReactor` to interpret the resource and instantiate the correct scene.
- **Structures as scenes** (chosen): stats and components live in the scene. No content resource ID needed for pre-placed structures — `EntityRegistryReactor` identifies them by their fixed `EntityId` assigned at match start. Runtime-spawned structures use `SpawnedSceneData` like any other spawned scene.

## Why Scenes

Structures are map-specific entities designed once and placed by level designers, not content authors producing many variants. A tower is not a template instantiated with different parameters — it is a specific scene with a specific set of components. Introducing `StructureData` would add a layer of indirection that serves no practical authoring benefit for a fixed set of structures.

Pre-placed structures (towers, inhibitors, nexus) are embedded in the map scene and registered at match start with stable `EntityId`s. Their live state (health, `IsDead`, status list) travels in `GameStateSnapshot` keyed by `EntityId` without a content resource ID. Clients identify them by their known fixed `EntityId`s from map load.

## Consequences

- Structures cannot be diffed or inspected as data resources. Stat values require opening scenes. Acceptable given the small, fixed structure set.
- Runtime-spawned structures (e.g. ability-placed turrets) go through `SpawnedSceneData` — consistent with all other runtime-spawned scenes, no special case needed.
