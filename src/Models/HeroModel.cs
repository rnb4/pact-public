using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Models;

[GlobalClass]
public partial class HeroModel : Model, IHasView
{
    [Export] public string Name { get; set; } = "Unnamed Hero";
    [Export] public PackedScene? ViewScene { get; set; }
    [Export(PropertyHint.File, "*.tres")] public required string Id { get; set; }

    [ExportGroup("Abilities")] 
    
    [Export] public AbilityModel? PassiveAbility { get; set; }
    [Export] public BasicAbilityModel? BasicAttack { get; set; }
    [Export] public RankedAbilityModel? Primary { get; set; }
    [Export] public RankedAbilityModel? Secondary { get; set; }
    [Export] public RankedAbilityModel? Tertiary { get; set; }
    [Export] public RankedAbilityModel? Ultimate { get; set; }

    
    [ExportGroup("Base Stats")] 
    
    [Export] public float MaxHealth { get; set; } = 500f;
    [Export] public float HealthRegen { get; set; } = 1f;
    [Export] public float MaxMana { get; set; } = 100f;
    [Export] public float ManaRegen { get; set; } = 1f;
    [Export] public float PhysicalDamage { get; set; } = 50f;
    [Export] public float MagicDamage { get; set; } = 50f;
    [Export] public float MoveSpeed { get; set; } = 4f;
    [Export] public float AttackSpeed { get; set; } = 0.75f;
    [Export] public float Armor { get; set; } = 1f;
    [Export] public float MagicResist { get; set; } = 1f;
    [Export] public float VisionRange { get; set; } = 5f;

    [ExportGroup("Stat Growth")] 
    
    [Export] public float HealthGrowth { get; set; } = 1f;
    [Export] public float PhysicalDamageGrowth { get; set; } = 1f;
    [Export] public float MagicDamageGrowth { get; set; } = 1f;
    [Export] public float MoveSpeedGrowth { get; set; } = 1f;
    [Export] public float AttackSpeedGrowth { get; set; } = 1f;
    [Export] public float ArmorGrowth { get; set; } = 1f;
    [Export] public float MagicResistGrowth { get; set; } = 1f;
    [Export] public float VisionRangeGrowth { get; set; }
}