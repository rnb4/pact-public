using Godot;

namespace Pact.Models;

[GlobalClass]
public partial class MinionModel : Resource
{
    [Export] public required HeroAbilityModel BasicAttack { get; set; }
    [Export] public int GoldValue { get; set; } = 10;
    [Export] public float ExperienceValue { get; set; } = 10;
    [Export] public float LeashRange { get; set; } = 3f;

    [ExportGroup("Base Stats")] [Export] public float Health { get; set; } = 100f;

    [Export] public float Damage { get; set; } = 10f;
    [Export] public float MoveSpeed { get; set; } = 10f;
    [Export] public float AttackSpeed { get; set; } = 0.75f;
    [Export] public float VisionRange { get; set; } = 4f;
}