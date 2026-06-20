using System.Collections.Generic;
using Godot;
using Pact.Models.Triggers;
using Pact.Server.Contexts;

namespace Pact.Models.Statuses;

[GlobalClass]
public partial class StatusModel : Model
{
    [Export] public string Name { get; set; } = "Unnamed Status";
    [Export] public float Duration { get; set; } = 1f;
    [Export] public TriggerModel[] Triggers { get; set; } = [];
    [Export] public required Texture2D Icon { get; set; }
    
    [ExportGroup("Stacks")] [Export] public int MaxStacks { get; set; } = 1;

    [Export] public required StackTimeoutBehaviorModel StackTimeout { get; set; }
    [Export] public StackApplicationBehavior StackApplication { get; set; }
}

public class Status
{
    public required StatusModel Model { get; init; }
    public required StackTimeoutBehavior StackTimeout { get; init; }
    public required StackApplicationBehavior StackApplication { get; init; }
    public int MaxStacks { get; init; }
    public float RemainingDuration { get; set; }
    public int CurrentStacks { get; set; } = 1;
}

public enum StackApplicationBehavior
{
    KeepDuration,
    RefreshDuration
}

public abstract record StackTimeoutBehavior;

public record RemoveAllStackTimeoutBehavior : StackTimeoutBehavior;

public record RemoveNStackTimeoutBehavior(int Amount) : StackTimeoutBehavior;

