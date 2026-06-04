namespace Pact.Shared.Messages.Results;

public enum RejectReason
{
    None = 0,
    Unauthorized,
    NotFound,
    IsDead,
    CannotAct,
    OnCooldown,
    InvalidTarget,
    AlreadyCasting,
    QueueFull,
    NotImplemented
}