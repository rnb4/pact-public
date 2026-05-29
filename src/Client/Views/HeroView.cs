using Godot;
using Pact.Shared.Snapshot;

namespace Pact.Client.Views;

[GlobalClass]
public partial class HeroView : View
{
    [ExportGroup("Animations")]
    [Export] public StringName RunAnimation { get; set; } = "run";
    [Export] public StringName WalkAnimation { get; set; } = "walk";
    [Export] public StringName IdleAnimation { get; set; } = "idle";
    [Export] public float RunThreshold = 64f;
    [Export] public float IdleThreshold = 0.5f;

    private FacingDirection _facing;
    private bool _isCasting;

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        base._Ready();
        Sprite.AnimationFinished += () => { _isCasting = false; };
    }

    public override void _Process(double delta)
    {
        if (!ClientRoot.Instance.TryGetHero(Id, out var entity)) return;

        SmoothedPosition = SmoothedPosition.Lerp(entity.Position, PositionSmoothing * (float)delta);
        MoveDelta = SmoothedPosition - GlobalPosition;
        GlobalPosition = SmoothedPosition;
        
        if (!Mathf.IsZeroApprox(entity.LookDirection.LengthSquared()))
            _facing = entity.LookDirection.ToFacingDirection();
        
        if (_isCasting && !Mathf.IsZeroApprox(entity.MoveDirection.LengthSquared()))
            _isCasting = false;

        if (_isCasting) return;

        if (MoveDelta.LengthSquared() > IdleThreshold)
            PlayWithFacing(entity.Speed > RunThreshold ? RunAnimation : WalkAnimation);
        else
            PlayWithFacing(IdleAnimation);
    }

    public void PlayCastAnimation(string animationName, FacingDirection facing)
    {
        _isCasting = true;
        _facing = facing;
        PlayWithFacing(animationName);
    }

    private void PlayWithFacing(StringName name, float speed = 1f)
    {
        Sprite.Play(name + "_" + _facing.ToString().ToLower(), speed);
    }
}