using Arch.Core;
using Godot;
using Pact.Models;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;

namespace Pact.Server.Operations;

public static class SpawnOp
{
    public static Entity SpawnHero(long peerId, HeroModel model, Vector2 position)
    {
        var hero = Game.Instance.World.Create(
            new Hero(1),
            new Position(position),
            new PathMove { Speed = model.MoveSpeed },
            new Pool<Health>(model.MaxHealth, model.MaxHealth, model.HealthRegen),
            new Pool<Mana>(model.MaxMana, model.MaxMana, model.ManaRegen),
            new Move(),
            new LookDirection(),
            new MetaData(model.ModelId, model.Name),
            new Statuses { Active = [] }
            );
        Game.Instance.EntityById[hero.Id] = hero;
        Game.Instance.Players.Register(peerId, hero);
        ApplyPassives(model, hero);
        return hero;
    }

    private static void ApplyPassives(HeroModel heroModel, Entity entity)
    {
        var source = new Source(entity.Id, 0, 0);
        foreach (var (ability, slot) in AbilityDomain.GetAbilities(heroModel))
            TriggerOp.Fire<OnAddTriggerModel>(ability, new Context(entity, entity, source with { AbilityIndex = slot}));
    }

    public static Entity? SpawnProjectile(ProjectileModel model, Entity caster, Entity target)
    {
        ref var casterPos = ref Game.Instance.World.Get<Position>(caster);
        var projectile = Game.Instance.World.Create(
            new Position(casterPos.Value),
            new MetaData(model.ModelId, string.Empty),
            new HomingProjectile(caster, target, model.OnHitEffect, model.Speed, model.Radius, model.Lifetime)
        );
        Game.Instance.EntityById[projectile.Id] = projectile;
        return projectile;
    }
}
