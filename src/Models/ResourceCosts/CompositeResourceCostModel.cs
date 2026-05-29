using Godot;

namespace Pact.Models.ResourceCosts;

[GlobalClass]
public partial class CompositeResourceCostModel : ResourceCostModel
{
    [Export] public ResourceCostModel[] Costs { get; set; } = [];
}