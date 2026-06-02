using Godot;
using Pact.Models.Effects;
using Pact.Models.Triggers;
using Pact.Server.Contexts;

namespace Pact.Models.Statuses;

[GlobalClass]
public partial class StatusModel : Model
{
    [Export] public string Name { get; set; } = "Unnamed Status";
    [Export] public float Duration { get; set; } = 1f;
    [Export] public required AbilityModel Ability { get; set; }
    [Export] public required Texture2D Icon { get; set; }
    
    [ExportGroup("Stacks")] [Export] public float MaxStacks { get; set; } = 1f;

    [Export] public required StackTimeoutBehaviorModel StackTimeout { get; set; }
    [Export] public StackApplicationBehavior StackApplication { get; set; }
}

public class Status
{
    public required Source Source { get; init; }
    public required StackTimeoutBehavior StackTimeout { get; init; }
    public required StackApplicationBehavior StackApplication { get; init; }
    public int MaxStacks { get; init; }
    public StatusStack[] CurrentStacks { get; init; } = [];
}

public record StatusStack(Source Source, int Value);

public enum StackApplicationBehavior
{
    KeepDuration,
    RefreshDuration
}

public abstract record StackTimeoutBehavior;

public record RemoveAllStackTimeoutBehavior : StackTimeoutBehavior;

public record RemoveNStackTimeoutBehavior(int Count) : StackTimeoutBehavior;

