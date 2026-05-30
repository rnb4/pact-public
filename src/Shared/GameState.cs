using System.Collections.Generic;
using Godot;
using Pact.Shared.State;
using Pact.Shared.State.Entities;
using EntityId = Pact.Shared.State.EntityId;
using HeroState = Pact.Shared.State.Entities.HeroState;

namespace Pact.Shared;

public record struct GameState()
{
    public readonly Dictionary<long, Vector2> CursorPositions = [];
    public readonly EntityTable<IEntity> Entities = new();

    public uint Tick= 0;

    public void Next()
    {
        Tick++;
    }
    
    public IEnumerable<(EntityId, T)> Query<T>() where T : IEntity
    {
        foreach (var (id, e) in Entities.All())
            if (e is T heroState)
                yield return (id, heroState);
    }

    public IEnumerable<(EntityId, HeroState)> Heroes()
    {
        return Query<HeroState>();
    }

    public IEnumerable<(EntityId, HomingProjectileState)> HomingProjectiles()
    {
        return Query<HomingProjectileState>();
    }
}