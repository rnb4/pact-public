using Godot;
using Pact.Server.Domains;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class ApplyStatModifierEffectModel : EffectModel
{
    [Export] public StatType StatType { get; set; }
    [Export] public StatOperation Operation { get; set; }
    [Export] public float Value { get; set; }
}