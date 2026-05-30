using System;
using Godot;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Models.Triggers;
using Pact.Shared.State.Entities;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Server.Operations;

public static class EffectOp
{
    public static void ApplyEffect(string modelId, EntityId casterId, ref HeroState target)
    {
        if (!Game.Instance.Models.TryGet<ProjectileModel>(modelId, out var model)) return;
        switch (model.OnHitEffect)
        {
            case DamageEffect dmg:
                target.Health = Math.Max(0, target.Health - dmg.Amount);
                GD.Print($"{target.Name} took {dmg.Amount} damage");
                Game.Instance.State.Entities.Set(target.EntityId, target);
                break;
            case SpawnEffect spawn:
            {
                if (!Game.Instance.State.Entities.TryGet(casterId, out var caster)) return;
                SpawnOp.SpawnProjectile(spawn.Projectile.ModelId, caster.Position, target.EntityId);
                break;
            }
        }
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, EntityId casterId, ref HeroState target)
    {
        switch (trigger.Effect)
        {
            case DamageEffect dmg:
                target.Health = Math.Max(0, target.Health - dmg.Amount);
                GD.Print($"{target.Name} took {dmg.Amount} damage");
                Game.Instance.State.Entities.Set(target.EntityId, target);
                break;
            case SpawnEffect spawn:
            {
                if (!Game.Instance.State.Entities.TryGet(casterId, out var caster)) return;
                SpawnOp.SpawnProjectile(spawn.Projectile.ModelId, caster.Position, target.EntityId);
                break;
            }
        }
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, EntityId casterId, Vector2 position)
    {
    }
}