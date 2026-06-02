using Arch.Core;
using Godot;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Models.Triggers;
using Pact.Server.Components;

namespace Pact.Server.Operations;

public static class EffectOp
{
    public static void ApplyOnHitEffect(long modelId, Entity caster, Entity target)
    {
        if (!Game.Instance.Models.TryGet<ProjectileModel>(modelId, out var model)) return;
        ApplyEffect(model.OnHitEffect, caster, target);
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, Entity caster, Entity target)
    {
        ApplyEffect(trigger.Effect, caster, target);
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, Entity caster, Vector2 position)
    {
    }

    private static void ApplyEffect(EffectModel effect, Entity caster, Entity target)
    {
        switch (effect)
        {
            case DamageEffect dmg:
                ApplyDamageEffect(dmg, caster, target);
                break;
            case SpawnEffect spawn:
                ApplySpawnEffect(spawn, caster, target);
                break;
        }
    }

    private static void ApplyDamageEffect(DamageEffect dmg, Entity caster, Entity target)
    {
        ref var targetHealth = ref Game.Instance.World.Get<Pool<Health>>(target);
        var damage = targetHealth.Deplete(dmg.Amount);
        var casterMeta = Game.Instance.World.Get<MetaData>(caster);
        var targetMeta = Game.Instance.World.Get<MetaData>(target);
        GD.Print($"{casterMeta.Name} did {damage} damage to {targetMeta.Name}");
    }

    private static void ApplySpawnEffect(SpawnEffect spawn, Entity caster, Entity target)
    {
        SpawnOp.SpawnProjectile(spawn.Projectile.ModelId, caster, target);
    }
}
