# Pact

A multiplayer MOBA prototype built in **Godot 4.7 / C#** with an authoritative dedicated server, an ECS simulation (Arch), and a data-driven ability system.

## Highlights

- **Authoritative server, thin client.** The server runs the whole simulation at a fixed 30 Hz tick. Clients only send inputs and render the snapshots they receive.
- **Custom snapshot network pipeline.** This replaces Godot's `MultiplayerSpawner`/`MultiplayerSynchronizer`. Each tick has one send point: an unreliable `GameStateSnapshot` that alone decides which entities exist, plus reliable one-shot server events. All of it is serialized with MemoryPack. Spawn and despawn recover on their own after packet loss.
- **Tick-boundary command pipeline.** Requests are pure data records. They queue up and run together at the start of each tick, so lower ping gives no advantage on contested actions. Validation guards reject bad requests with typed `Result` reasons, which the client turns into UI feedback.
- **ECS simulation.** Server entities live in an [Arch](https://github.com/genaray/Arch) `World`. Systems run in an explicit order each tick: projectiles → movement → casting → action queue → cooldowns → statuses → regen.
- **One ability system for everything.** Hero skills, basic attacks, items and statuses all compose the same parts: _Triggers_ (on-activate, on-add/remove, interval, query) fire _Effects_ (damage, status, stat modifier, spawn, composite), gated by _Resource Costs_.

## Layout

```
src/
  Server/   Game loop, ECS components & systems, request handlers, domains (cast, cooldown, stats)
  Client/   Views, UI, input/camera, server-message handlers
  Shared/   Network messages, snapshot state, shared components — used by both sides
  Models/   Data-driven content: abilities, triggers, effects, statuses, resource costs
docs/adr/   Architecture Decision Records
tools/      Tiled map import post-processing, sprite-frame loader
```

## Running

Requires Godot 4.7 (.NET) and the .NET 8 SDK.

```sh
godot --headless        # dedicated server on :4444
godot                   # client, connects to 127.0.0.1:4444
```

Art assets are not in the repo.
