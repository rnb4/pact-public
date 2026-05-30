using Godot;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class CompositeEffectModel : EffectModel
{
    [Export] public EffectModel[] Effects { get; set; } = [];
}