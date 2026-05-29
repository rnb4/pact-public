namespace Pact.Gameplay.Statuses;
// TODO: data driven statuses

public abstract record StackTimeoutBehavior;

public record RemoveAllStackTimeoutBehavior : StackTimeoutBehavior;

public record RemoveNStackTimeoutBehavior(int Count) : StackTimeoutBehavior;
