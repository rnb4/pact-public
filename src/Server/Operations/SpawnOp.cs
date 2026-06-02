using Arch.Core;
using Godot;
using Pact.Models;
using Pact.Server.Components;

namespace Pact.Server.Operations;

public static class SpawnOp
{
    public static Entity SpawnHero(long peerId, HeroModel model, Vector2 position)
    {
        var hero = Game.Instance.World.Create(
            new Hero(),
            new Position(position),
            new PathMove { Speed = model.MoveSpeed },
            new Pool<Health>(model.MaxHealth, model.MaxHealth, model.HealthRegen),
            new Pool<Mana>(model.MaxMana, model.MaxMana, model.ManaRegen),
            new Move(),
            new LookDirection(),
            new MetaData(model.ModelId, model.Name)
            );
        Game.Instance.EntityById[hero.Id] = hero;
        Game.Instance.Players.Register(peerId, hero);
        return hero;
    }

    public static Entity? SpawnProjectile(long modelId, Entity caster, Entity target)
    {
        if (!Game.Instance.Models.TryGet<ProjectileModel>(modelId, out var model)) return null;

        ref var casterPos = ref Game.Instance.World.Get<Position>(caster);
        var projectile = Game.Instance.World.Create(
            new HomingProjectile(),
            new Position(casterPos.Value),
            new MetaData(modelId, string.Empty),
            new HomingData(caster, target, model.Speed, model.Radius, model.Lifetime)
        );
        Game.Instance.EntityById[projectile.Id] = projectile;
        return projectile;
    }
}
