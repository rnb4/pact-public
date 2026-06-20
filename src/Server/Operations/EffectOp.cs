using System;
using System.Linq;
using Arch.Core;
using Godot;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Models.Statuses;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Shared.State;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Shared.Messages.Results;

namespace Pact.Server.Operations;

public static class EffectOp
{
    public static Result Apply(EffectModel effect, Context context) => effect switch
    {
        ApplyStatModifierEffectModel applyStatModifierEffectModel => ApplyStatModifierEffect(applyStatModifierEffectModel, context),
        ApplyStatusEffectModel applyStatusEffectModel => ApplyStatusEffect(applyStatusEffectModel, context),
        CompositeEffectModel compositeEffectModel => ApplyCompositeEffect(compositeEffectModel, context),
        DamageEffectModel damageEffectModel => ApplyDamageEffect(damageEffectModel, context),
        SpawnEffectModel spawnEffectModel => ApplySpawnEffect(spawnEffectModel, context),
        _ => Result.NotImplemented,
    };

    private static Result ApplyDamageEffect(DamageEffectModel dmg, Context context)
    {
        if (!context.Target.IsEntity) return Result.InvalidTarget;
        ref var health = ref Game.Instance.World.Get<Pool<Health>>(context.Target.Entity);
        var damage = health.Deplete(dmg.Amount);
        var casterMeta = Game.Instance.World.Get<MetaData>(context.Caster);
        var targetMeta = Game.Instance.World.Get<MetaData>(context.Target.Entity);
        GD.Print($"{casterMeta.Name} did {damage} damage to {targetMeta.Name}");
        return Result.Ok;
    }
    
    private static Result ApplySpawnEffect(SpawnEffectModel spawn, Context context)
    {
        if (!Game.Instance.Models.TryGet<ProjectileModel>(spawn.Projectile.ModelId, out var model)) return Result.NotFound;
        if (!context.Target.IsEntity) return Result.InvalidTarget;
        SpawnOp.SpawnProjectile(model, context.Caster, context.Target.Entity);
        return Result.Ok;
    }

    private static Result ApplyCompositeEffect(CompositeEffectModel composite, Context context)
    {
        foreach (var effect in composite.Effects)
            if (Apply(effect, context) is {} result && result != Result.Ok) return result;
        return Result.Ok;
    }
    
    private static Result ApplyStatusEffect(ApplyStatusEffectModel effect, Context context)
    {
        if (!context.Target.IsEntity) return Result.InvalidTarget;
        ref var statuses = ref Game.Instance.World.Get<Statuses>(context.Target.Entity);
        foreach (var activeStatus in statuses.Active.Where(activeStatus => activeStatus.Model == effect.Status))
        {
            activeStatus.CurrentStacks = Math.Min(activeStatus.CurrentStacks + 1, activeStatus.MaxStacks);
            if (activeStatus.StackApplication == StackApplicationBehavior.RefreshDuration)
                activeStatus.RemainingDuration = effect.Status.Duration;
            return Result.Ok;
        }
        var status = new Status
        {
            Model = effect.Status,
            StackTimeout = effect.Status.StackTimeout switch
            {
                RemoveNStacks removeNStacks => new RemoveNStackTimeoutBehavior(removeNStacks.Amount),
                _ => new RemoveAllStackTimeoutBehavior(),
            },
            StackApplication = effect.Status.StackApplication,
            MaxStacks = effect.Status.MaxStacks,
            RemainingDuration = effect.Status.Duration,
        };
        statuses.Active.Add(status);
        TriggerOp.Fire<OnAddTriggerModel>(effect.Status.Triggers, context);
        return Result.Ok;
    }

    private static Result ApplyStatModifierEffect(ApplyStatModifierEffectModel effect, Context context)
    {
        if (!context.Target.IsEntity) return Result.InvalidTarget;
        StatDomain.AddModifier(context.Target.Entity, new StatModifier
        {
            StatType = effect.StatType,
            Operation = effect.Operation,
            Value = effect.Value
        }, context.Source);
        return Result.Ok;
    }
}
