using Godot;
using Pact.Models.Effects;
using Pact.Models.ResourceCosts;
using Pact.Shared.State;

namespace Pact.Models.Triggers;

[GlobalClass]
public abstract partial class TriggerModel : Resource
{
    [Export] public required EffectModel Effect { get; set; }
    [Export] public ResourceCostModel? Cost { get; set; }
}