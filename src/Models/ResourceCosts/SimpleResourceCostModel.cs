using Godot;

namespace Pact.Models.ResourceCosts;

[GlobalClass]
public partial class SimpleResourceCostModel : ResourceCostModel
{
    [Export] public float Cost { get; set; }
}

[GlobalClass]
public partial class HealthCost : SimpleResourceCostModel;

[GlobalClass]
public partial class ManaCost : SimpleResourceCostModel;