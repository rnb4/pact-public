using Godot;
using Pact.Models;
using Pact.Server.Repos;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;

namespace Pact.Server.Operations;

public static class SpawnOp
{
    public static EntityId? SpawnHero(long peerId, string modelId, Vector2 position)
    {
        if (!Runtime.Instance.ModelRepo.TryGet<HeroModel>(modelId, out var model)) return null;
        var hero = model.ToState();
        hero.ModelId = model.Id;
        hero.Position = position;
        var id = Runtime.Instance.StateRepo.Heroes.Allocate(hero);
        hero.EntityId = id;
        Runtime.Instance.StateRepo.Heroes.Set(id, hero);
        Runtime.Instance.PlayerRepo.Register(peerId, hero.EntityId);
        return id;
    }

    public static EntityId? SpawnProjectile(string modelId, Vector2 position)
    {
        if (!Runtime.Instance.ModelRepo.TryGet<ProjectileModel>(modelId, out var model)) return null;
        var projectile = model.ToState();
        projectile.Position = position;
        projectile.ModelId = modelId;
        var id = Runtime.Instance.StateRepo.Projectiles.Allocate(projectile);
        projectile.EntityId = id;
        Runtime.Instance.StateRepo.Projectiles.Set(id, projectile);
        return id;
    }

}