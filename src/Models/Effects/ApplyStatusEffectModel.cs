using Godot;
using Pact.Models.Statuses;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class ApplyStatusEffectModel : EffectModel
{
    [Export] public required StatusModel Status { get; set; }
}
