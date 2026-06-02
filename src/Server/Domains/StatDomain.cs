using System.Collections.Generic;
using Arch.Core;

namespace Pact.Server.Domains;

public static class StatDomain
{
    private static Dictionary<Entity, StatModifier> _modifiers = new();

    public static float Compute(Entity entity, StatType statType)
    {
        return 0f;
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
