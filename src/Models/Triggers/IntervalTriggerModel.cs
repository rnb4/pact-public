using Godot;

namespace Pact.Models.Triggers;

[GlobalClass]
public partial class IntervalTriggerModel : TriggerModel
{
    [Export] public int Interval { get; set; } = 1;
}