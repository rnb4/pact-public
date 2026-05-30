namespace Pact.Shared.State;

public enum TriggerType : byte
{
    OnAdd,
    OnRemove,
    OnActivate,
    OnQuery,
    OnInterval,
    OnEvent
}