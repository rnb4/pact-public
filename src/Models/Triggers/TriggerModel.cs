using Godot;
using Pact.Models.ResourceCosts;
using Pact.Shared.State;

namespace Pact.Models.Triggers;

[GlobalClass]
public abstract partial class TriggerModel : Resource
{
    [Export] public required Effects.EffectModel Effect { get; set; }
    [Export] public ResourceCostModel? Cost { get; set; }
    public abstract TriggerType TriggerType { get; }
}