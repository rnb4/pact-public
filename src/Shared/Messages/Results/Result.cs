namespace Pact.Shared.Messages.Results;

public class Result
{
    public RejectReason Reason { get; }
    public bool IsValid => Reason == RejectReason.None;
    private Result(RejectReason reason) { Reason = reason; }
    public override string ToString() => Reason.ToString();
    
    public static readonly Result Ok = new(RejectReason.None);
    public static readonly Result Unauthorized = new(RejectReason.Unauthorized);
    public static readonly Result HeroNotFound = new(RejectReason.HeroNotFound);
    public static readonly Result AbilityNotFound = new(RejectReason.AbilityNotFound);
    public static readonly Result IsDead = new(RejectReason.IsDead);
    public static readonly Result CannotAct = new(RejectReason.CannotAct);
    public static readonly Result OnCooldown = new(RejectReason.OnCooldown);
    public static readonly Result InvalidTarget = new(RejectReason.InvalidTarget);
    public static readonly Result AlreadyCasting = new(RejectReason.AlreadyCasting);
    public static readonly Result QueueFull = new(RejectReason.QueueFull);
    public static readonly Result NotImplemented = new(RejectReason.NotImplemented);
}