using Godot;
using Pact.Server.Contexts;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class DamageEffect : EffectModel
{
    [Export] public DamageType DamageType { get; set; }
    [Export] public float Amount { get; set; }
}