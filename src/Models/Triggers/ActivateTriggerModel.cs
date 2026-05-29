using Godot;
using Pact.Shared.State;

namespace Pact.Models.Triggers;

[GlobalClass]
public partial class ActivateTriggerModel : TriggerModel
{
    [Export] public string? AnimationName { get; set; }
    [Export] public int FireFrame { get; set; }

    public override TriggerType TriggerType { get; } = TriggerType.OnActivate;
}