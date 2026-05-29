# Command Pipeline: Pure Data Commands, Per-Tick Queue, Dual Rejection

Player inputs are represented as pure MemoryPack-serializable `IClientCommand` records with no logic. A `CommandDispatcher` routes each command to a typed `IClientCommandHandler<T>` on the server. Commands are collected into a per-player queue and processed together at the tick boundary — not immediately on arrival — so that no player gains an advantage from lower ping on contested actions like simultaneous ability casts.

Rejection is handled two ways simultaneously: the server fires an explicit rejection RPC to the originating client for immediate UI feedback (insufficient mana, invalid target, etc.), and the client's optimistic movement prediction is corrected by authoritative state sync if it diverges. No commands are predicted client-side — all entities including the local Hero lerp to server position. Server-side pathfinding makes client movement reproduction impractical, so rejections are purely server-driven and visual correction is via lerp.

## Considered Options

- **Immediate command processing**: simpler, but creates a ping advantage on contested actions.
- **Single rejection mechanism**: explicit RPC alone gives fast feedback but doesn't handle edge cases from optimistic prediction; state sync alone gives no actionable UI feedback to the player.
