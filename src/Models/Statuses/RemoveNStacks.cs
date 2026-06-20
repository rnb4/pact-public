using Godot;

namespace Pact.Models.Statuses;

[GlobalClass]
public partial class RemoveNStacks : StackTimeoutBehaviorModel
{
    [Export] public int Amount { get; set; }
}
