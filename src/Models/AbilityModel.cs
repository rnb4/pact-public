using Godot;
using Pact.Models.Triggers;

namespace Pact.Models;

[GlobalClass]
public partial class AbilityModel : Model
{
    [Export] public float Cooldown = 1f;
    [Export] public float Range = 5f;
    [Export] public TargetType TargetType;
    [Export] public TriggerModel[] Triggers { get; set; } = [];
}

public enum TargetType
{
    Ground,
    Entity
}