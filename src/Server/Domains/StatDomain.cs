using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Server.Contexts;

namespace Pact.Server.Domains;

public static class StatDomain
{
    private static readonly Dictionary<Entity, List<(StatModifier Modifier, Source Source)>> CachedModifiers = new();

    public static void AddModifier(Entity target, StatModifier modifier, Source source)
    {
        if (!CachedModifiers.TryGetValue(target, out var list))
        {
            list = [];
            CachedModifiers[target] = list;
        }
        list.Add((modifier, source));
        UpdateEntityComponents(target);
    }

    public static void RemoveModifiers(Entity target, Source source)
    {
        if (!CachedModifiers.TryGetValue(target, out var list)) return;
        list.RemoveAll(entry => entry.Source == source);
        UpdateEntityComponents(target);
    }

    public static float Compute(Entity entity, StatType statType)
    {
        var meta = Game.Instance.World.Get<MetaData>(entity);
        if (!Game.Instance.Models.TryGet<HeroModel>(meta.ModelId, out var model)) return 0f;
        var baseValue = GetBaseStat(model, statType);
        var mods = CachedModifiers.TryGetValue(entity, out var list)
            ? list.Where(e => e.Modifier.StatType == statType).Select(e => e.Modifier)
            : [];
        return ApplyModifiers(baseValue, mods);
    }

    public static float ComputeInstant(Entity entity, StatType statType, Context context)
    {
        var cached = Compute(entity, statType);
        var meta = Game.Instance.World.Get<MetaData>(entity);
        if (!Game.Instance.Models.TryGet<HeroModel>(meta.ModelId, out var model)) return cached;

        var instantMods = new List<StatModifier>();

        foreach (var (ability, slot) in AbilityDomain.GetAbilities(model))
            CollectQueryModifiers(ability.Triggers, statType, context, instantMods);

        var world = Game.Instance.World;
        if (world.Has<Statuses>(entity))
        {
            ref var statuses = ref world.Get<Statuses>(entity);
            foreach (var status in statuses.Active)
                CollectQueryModifiers(status.Model.Ability.Triggers, statType, context, instantMods);
        }

        return ApplyModifiers(cached, instantMods);
    }

    private static void CollectQueryModifiers(
        IEnumerable<TriggerModel> triggers,
        StatType statType,
        Context context,
        List<StatModifier> output)
    {
        foreach (var trigger in triggers)
        {
            if (trigger is not QueryTriggerModel query) continue;
            if (!query.Predicate(context)) continue;
            if (trigger.Effect is ApplyStatModifierEffectModel { StatType: var st, Operation: var op, Value: var val } && st == statType)
                output.Add(new StatModifier { StatType = statType, Operation = op, Value = val });
        }
    }

    private static float GetBaseStat(HeroModel model, StatType stat) => stat switch
    {
        StatType.MaxHealth => model.MaxHealth,
        StatType.HealthRegen => model.HealthRegen,
        StatType.MaxMana => model.MaxMana,
        StatType.ManaRegen => model.ManaRegen,
        StatType.PhysicalDamage => model.PhysicalDamage,
        StatType.MagicDamage => model.MagicDamage,
        StatType.MoveSpeed => model.MoveSpeed,
        StatType.AttackSpeed => model.AttackSpeed,
        StatType.Armor => model.Armor,
        StatType.MagicResist => model.MagicResist,
        StatType.AbilityRange => 0f,
        StatType.AbilitySize => 0f,
        StatType.VisionRange => model.VisionRange,
        _ => 0f
    };

    private static float ApplyModifiers(float baseValue, IEnumerable<StatModifier> modifiers)
    {
        var flat = 0f;
        var percentAdd = 0f;
        var percentMultiply = 1f;
        foreach (var mod in modifiers)
        {
            switch (mod.Operation)
            {
                case StatOperation.Flat: flat += mod.Value; break;
                case StatOperation.PercentAdd: percentAdd += mod.Value; break;
                case StatOperation.PercentMultiply: percentMultiply *= 1f + mod.Value; break;
            }
        }
        return (baseValue + flat) * (1f + percentAdd) * percentMultiply;
    }

    private static void UpdateEntityComponents(Entity entity)
    {
        var world = Game.Instance.World;
        if (world.Has<Pool<Health>>(entity))
        {
            ref var health = ref world.Get<Pool<Health>>(entity);
            health.Max = Compute(entity, StatType.MaxHealth);
            if (health.Current > health.Max) health.Current = health.Max;
        }
        if (world.Has<Pool<Mana>>(entity))
        {
            ref var mana = ref world.Get<Pool<Mana>>(entity);
            mana.Max = Compute(entity, StatType.MaxMana);
            if (mana.Current > mana.Max) mana.Current = mana.Max;
        }
        if (world.Has<PathMove>(entity))
        {
            ref var move = ref world.Get<PathMove>(entity);
            move.Speed = Compute(entity, StatType.MoveSpeed);
        }
    }
}

public struct StatModifier
{
    public StatType StatType;
    public StatOperation Operation;
    public float Value;
}

public enum StatType
{
    MaxHealth,
    HealthRegen,
    MaxMana,
    ManaRegen,
    PhysicalDamage,
    MagicDamage,
    MoveSpeed,
    AttackSpeed,
    Armor,
    MagicResist,
    AbilityRange,
    AbilitySize,
    VisionRange,
}

public enum StatOperation { Flat, PercentAdd, PercentMultiply }
