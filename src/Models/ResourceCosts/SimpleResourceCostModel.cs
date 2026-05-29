using Godot;
using Pact.Gameplay.Resources;
using Pact.Gameplay.Resources.Costs;

namespace Pact.Models.ResourceCosts;

[GlobalClass]
public partial class SimpleResourceCostModel : ResourceCostModel
{
    
    [Export] public ResourceType Type { get; set; }
    [Export] public float Cost { get; set; }
}