using Godot;
using Pact.Shared.State;
using Source = Pact.Server.Contexts.Source;

namespace Pact.Gameplay.Statuses;
// TODO: data driven statuses

public class Status
{
    public required Source Source { get; init; }
    public AbilitySlot? Ability { get; init; }
    public required Texture2D Icon { get; init; }
    public required StackTimeoutBehavior StackTimeout { get; init; }
    public required StackApplicationBehavior StackApplication { get; init; }
    public int MaxStacks { get; init; }
    public int CurrentStacks { get; init; }
}