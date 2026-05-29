namespace Pact.Shared.Messages.Results;

public enum RejectReason
{
    None = 0,
    Unauthorized,
    HeroNotFound,
    AbilityNotFound,
    IsDead,
    CannotAct,
    OnCooldown,
    InvalidTarget,
    AlreadyCasting,
    QueueFull,
    NotImplemented
}