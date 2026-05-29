# Custom Snapshot Network Pipeline over Godot's Built-in Multiplayer Tools

We replaced Godot's `MultiplayerSpawner` and `MultiplayerSynchronizer` with a custom snapshot pipeline. The server transmits a `GameStateSnapshot` (unreliable) and `ServerEvents` (reliable) to each client every tick via two RPCs on `NetworkManager`. Clients receive inputs via a single `SubmitCommands` RPC and rejections via `ReceiveCommandRejection`.

The `GameStateSnapshot` is the sole authority for entity existence — entities present in the snapshot exist, entities absent are despawned. Each entry carries both config and live state, so spawn and despawn require no separate event types and the system is self-healing under packet loss. `ServerEvents` carries one-shot `IGameEvent`s that cannot be reconstructed from state and must not be lost.

Godot's built-in tools were rejected because they create three uncoordinated communication channels (spawner, synchronizer, ad-hoc RPCs) with no single point of fog-of-war filtering or tick-boundary control. The custom pipeline provides one authoritative send point per tick, deterministic ordering, and uniform MemoryPack serialization throughout.

## Consequences

- Entity registry entries carry config fields every tick. Acceptable given their small size.
- Spawn and despawn are delayed by at most one tick on packet loss. Acceptable for a MOBA tick rate.
- Fog of war filtering, sequence numbering, and reliability decisions are all owned by this pipeline — there is no Godot fallback if they are implemented incorrectly.
