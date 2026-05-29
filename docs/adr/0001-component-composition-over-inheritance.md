# Component Composition Over Inheritance for Entities

The game has four entity families (Heroes, Minions, Monsters, Structures) that share some behavior but diverge sharply — Structures don't move, Heroes have player input, Minions have simple AI, Monsters have complex AI with leashing. A deep inheritance hierarchy would force awkward multi-inheritance or empty method overrides the moment a new entity type needs a combination of traits that doesn't fit the tree. Instead, `Unit` (`CharacterBody3D`) is a behavior-free container; all behavior is provided by assembled child Component nodes. New entity types are created by composing Components in the Godot editor without writing new classes.

## Considered Options

- **Inheritance hierarchy** (`Unit → Character → Hero/Minion`, `Unit → Structure`): simpler to start but rigid — breaks down when an entity needs abilities but no movement, or movement but no AI.
- **Pure ECS**: would lose Godot editor integration and scene-based content authoring, which is central to the content creation workflow.
