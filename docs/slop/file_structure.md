pact/
  ├── project.godot
  ├── Pact.csproj
  ├── docs/
  │   └── adr/
  │
  ├── assets/                         # Raw imported assets
  │   ├── textures/
  │   ├── audio/
  │   └── vfx/
  │
  ├── scenes/                         # Godot scenes (.tscn)
  │   ├── entities/
  │   │   ├── hero.tscn
  │   │   ├── minion.tscn
  │   │   ├── monster.tscn
  │   │   └── spawned/                # Projectile, aura, AoE templates
  │   ├── structures/                 # tower.tscn, inhibitor.tscn, nexus.tscn
  │   ├── map/
  │   └── ui/
  │       ├── hud.tscn
  │       └── shop.tscn
  │
  ├── resources/                      # Godot resources (.tres instances)
  │   ├── heroes/                     # HeroData
  │   ├── minions/                    # MinionData (including super_minion)
  │   ├── monsters/                   # MonsterData
  │   ├── abilities/                  # AbilityData + AbilityRank
  │   ├── behaviors/                  # Behavior .tres (Trigger + Effect pairings)
  │   ├── predicates/                 # Predicate .tres
  │   ├── statuses/                   # StatusData
  │   ├── items/                      # ItemData
  │   └── vfx/                        # VisualEffectData
  │
  └── src/                            # C# source (.cs class definitions)
      ├── Autoload/                   # NetworkManager
      │
      ├── Shared/                     # MemoryPack types used by both sides
      │   ├── Commands/               # IClientCommand, MoveCommand, CastCommand, etc.
      │   ├── Events/                 # IGameEvent, VfxEvent, AttackEvent, etc.
      │   └── Snapshot/               # GameStateSnapshot, ServerEvents, EntityId, FacingDirection, Team
      │
      ├── Gameplay/                   # Simulation types (server runs these; client loads resources)
      │   ├── Entities/               # Unit, Component base, HeroData, MinionData, MonsterData
      │   ├── Components/             # AbilitySet, AbilityHandler, AttackComponent
      │   ├── Abilities/              # Behavior, Trigger, Effect, Predicate base + AbilityData, AbilityRank
      │   ├── Triggers/               # ActiveTrigger, EventTrigger, ContinuousTrigger, ConditionTrigger, QueryTrigger
      │   ├── Effects/                # DamageEffect, ApplyStatusEffect, RemoveStatusEffect, DamageModifierEffect, etc.
      │   ├── Predicates/             # HealthBelowThreshold, HasStatus, IsInRange, AndPredicate, etc.
      │   ├── Stats/                  # Stat, StatModifier, ResourcePool, ResourceType, Source
      │   ├── Status/                 # StatusData, StatusHandle, StatusStack, StackExpiry, StackApplication, ImpairmentType
      │   ├── Damage/                 # DamageInstance, DamageType
      │   ├── Economy/                # ItemData, Gold logic
      │   └── Spawned/                # SpawnedSceneData, MotionComponent, HitboxComponent
      │
      ├── Server/                     # Server-only systems
      │   ├── Systems/                # GameState, StatusManager, DamageCalculator, SpawnManager,
      │   │                           #   WaveSpawner, FogOfWar, WinCondition, NexusProtectionSystem,
      │   │                           #   RespawnTimer, CampRespawnTimer, InhibitorRespawnTimer
      │   ├── Handlers/               # IClientCommandHandler<T> per command type
      │   └── AI/                     # BehaviorTree, MinionAI, MonsterAI, TowerAI
      │
      └── Client/                     # Client-only systems
          ├── Systems/                # EntityRegistryReactor, InputManager, ClientGameEventBus, EffectSpawner
          ├── Views/                  # EntityView
          └── UI/                     # HUD, Shop scripts

  Key rules:
  - Shared/ — anything MemoryPack-serialized crossing the network wire
  - Gameplay/ — simulation types; server executes them, client only loads the resource classes
  - Server/ — systems that only ever run on the dedicated server
  - Client/ — systems that only ever run on a player's machine
  - resources/ — .tres instances; src/ — the C# class definitions they instantiate