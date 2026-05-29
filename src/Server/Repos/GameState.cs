using System.Collections.Generic;
using Godot;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;
using HeroState = Pact.Shared.State.Entities.HeroState;

namespace Pact.Server.Repos;

public class GameState
{
    public uint Tick { get; set; }
    public readonly EntityTable<HeroState> Heroes = new();
    public readonly EntityTable<HomingProjectileState> Projectiles = new();
    public readonly Dictionary<long, Vector2> CursorPositions = [];
}