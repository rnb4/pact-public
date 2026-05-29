using System.Collections.Generic;
using Godot;
using Pact.Shared.Snapshot;
using Pact.Shared.State;
using Pact.Shared.State.Entities;
using HeroState = Pact.Shared.State.Entities.HeroState;

namespace Pact.Server.Repos;

public class GameState
{
    public uint Tick { get; set; }
    public readonly EntityTable<Entity> Entities = new();
    public readonly Dictionary<long, Vector2> CursorPositions = [];

    public IEnumerable<(EntityId, T)> Query<T>() where T : Entity
    {
        foreach (var (id, e) in Entities.All())
            if (e is T heroState)
                yield return (id, heroState);
    }
    
    public IEnumerable<(EntityId, HeroState)> Heroes() => Query<HeroState>();
    public IEnumerable<(EntityId, HomingProjectileState)> HomingProjectiles() => Query<HomingProjectileState>();
}