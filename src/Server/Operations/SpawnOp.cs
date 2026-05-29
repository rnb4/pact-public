using Godot;
using Pact.Models;
using Pact.Shared.Snapshot;

namespace Pact.Server.Operations;

public static class SpawnOp
{
    public static EntityId? SpawnHero(long peerId, string modelId, Vector2 position)
    {
        if (!Game.Instance.Models.TryGet<HeroModel>(modelId, out var model)) return null;
        var hero = model.ToState();
        hero.ModelId = model.ModelId;
        hero.Position = position;
        var id = Game.Instance.State.Entities.Allocate(hero);
        hero.EntityId = id;
        Game.Instance.State.Entities.Set(id, hero);
        Game.Instance.Players.Register(peerId, hero.EntityId);
        return id;
    }

    public static EntityId? SpawnProjectile(string modelId, Vector2 position, EntityId targetId)
    {
        
        if (!Game.Instance.Models.TryGet<ProjectileModel>(modelId, out var model)) return null;
        var projectile = model.ToState();
        projectile.Position = position;
        projectile.ModelId = modelId;
        projectile.TargetId = targetId;
        var id = Game.Instance.State.Entities.Allocate(projectile);
        projectile.EntityId = id;
        Game.Instance.State.Entities.Set(id, projectile);
        return id;
    }

}