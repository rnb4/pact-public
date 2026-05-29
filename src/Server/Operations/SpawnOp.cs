using Godot;
using Pact.Models;
using Pact.Shared.Snapshot;

namespace Pact.Server.Operations;

public static class SpawnOp
{
    public static EntityId? SpawnHero(long peerId, string modelId, Vector2 position)
    {
        if (!Runtime.Instance.ModelRepo.TryGet<HeroModel>(modelId, out var model)) return null;
        var hero = model.ToState();
        hero.ModelId = model.ModelId;
        hero.Position = position;
        var id = Runtime.Instance.GameState.Entities.Allocate(hero);
        hero.EntityId = id;
        Runtime.Instance.GameState.Entities.Set(id, hero);
        Runtime.Instance.PlayerRepo.Register(peerId, hero.EntityId);
        return id;
    }

    public static EntityId? SpawnProjectile(string modelId, Vector2 position, EntityId targetId)
    {
        
        if (!Runtime.Instance.ModelRepo.TryGet<ProjectileModel>(modelId, out var model)) return null;
        var projectile = model.ToState();
        projectile.Position = position;
        projectile.ModelId = modelId;
        projectile.TargetId = targetId;
        var id = Runtime.Instance.GameState.Entities.Allocate(projectile);
        projectile.EntityId = id;
        Runtime.Instance.GameState.Entities.Set(id, projectile);
        return id;
    }

}