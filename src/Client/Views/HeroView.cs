using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Client.Views;

[GlobalClass]
public partial class HeroView : View
{
    private FacingDirection _facing;
    private bool _isCasting;

    private Vector2 _moveDelta;
    [Export] public float IdleThreshold = 0.5f;
    [Export] public float RunThreshold = 64f;

    [ExportGroup("Animations")] [Export] public StringName RunAnimation { get; set; } = "run";

    [Export] public StringName WalkAnimation { get; set; } = "walk";
    [Export] public StringName IdleAnimation { get; set; } = "idle";

    public override void _Ready()
    {
        base._Ready();
        Sprite.AnimationFinished += () => { _isCasting = false; };
        var material = new ShaderMaterial();
        material.Shader = GD.Load<Shader>("res://assets/shaders/outline.gdshader");
        Sprite.Material = material;
    }

    public override void _Process(double delta)
    {
        if (!ClientRoot.Instance.GameState.Entities.TryGet(Id, out var e) || e is not HeroState entity) return;

        SmoothedPosition = SmoothedPosition.Lerp(entity.Position, PositionSmoothing * (float)delta);
        _moveDelta = SmoothedPosition - GlobalPosition;
        GlobalPosition = SmoothedPosition;

        if (!Mathf.IsZeroApprox(entity.LookDirection.LengthSquared()))
            _facing = entity.LookDirection.ToFacingDirection();

        if (_isCasting && !Mathf.IsZeroApprox(entity.MoveDirection.LengthSquared()))
            _isCasting = false;

        if (_isCasting) return;

        if (_moveDelta.LengthSquared() > IdleThreshold)
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

    public void Highlight()
    {
        Sprite.Material.Set("shader_parameter/width", 1);
    }

    public void UnHighlight()
    {
        Sprite.Material.Set("shader_parameter/width", 0);
    }
}