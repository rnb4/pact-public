using Godot;

namespace Pact.Models.Statuses;

public partial class StackTimeoutBehaviorModel : Resource;

[GlobalClass]
public partial class RemoveAllStacks : StackTimeoutBehaviorModel;

[GlobalClass]
public partial class RemoveNStacks : StackTimeoutBehaviorModel
{
    [Export] public int Amount { get; set; }
}