using Godot;
using Pact.Shared.State;

namespace Pact.Models.Triggers;

[GlobalClass]
public partial class IntervalTriggerModel : TriggerModel
{
    [Export] public int Interval { get; set; } = 1;
    public override TriggerType TriggerType { get; } = TriggerType.OnInterval;
}