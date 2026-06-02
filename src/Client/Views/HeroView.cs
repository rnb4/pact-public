using System;
using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Client.Views;

[GlobalClass]
public partial class HeroView : View
{
    private FacingDirection _facing;
    private bool _isCasting;

    private Vector2 _moveDelta;
    private FancyProgressBar _healthBar = null!;
    
    [Export] public float IdleThreshold = 0.5f;
    [Export] public float RunThreshold = 64f;

    [ExportGroup("Animations")] 
    
    [Export] public StringName RunAnimation { get; set; } = "run";
    [Export] public StringName WalkAnimation { get; set; } = "walk";
    [Export] public StringName IdleAnimation { get; set; } = "idle";

    public override void _Ready()
    {
        base._Ready();
        Sprite.AnimationFinished += () => { _isCasting = false; };
        var material = new ShaderMaterial();
        material.Shader = GD.Load<Shader>("res://assets/shaders/outline.gdshader");
        Sprite.Material = material;
        _healthBar = GetNode<FancyProgressBar>("%HealthBar");
    }

    public override void _Process(double delta)
    {
        if (!ClientRoot.Instance.GameState.TryGet<HeroState>(Id, out var hero)) return;

        SmoothedPosition = SmoothedPosition.Lerp(hero.Position, PositionSmoothing * (float)delta);
        _moveDelta = SmoothedPosition - GlobalPosition;
        GlobalPosition = SmoothedPosition;

        if (!Mathf.IsZeroApprox(hero.LookDirection.LengthSquared()))
            _facing = hero.LookDirection.ToFacingDirection();

        if (_isCasting && !Mathf.IsZeroApprox(hero.MoveDirection.LengthSquared()))
            _isCasting = false;

        if (_isCasting) return;

        if (_moveDelta.LengthSquared() > IdleThreshold)
            PlayWithFacing(hero.Speed > RunThreshold ? RunAnimation : WalkAnimation);
        else
            PlayWithFacing(IdleAnimation);
        
        if (Math.Abs(hero.MaxHealth - _healthBar.Max) > 0.01f) _healthBar.Max = hero.MaxHealth;
        if (Math.Abs(hero.Health - _healthBar.Current) > 0.01f) _healthBar.Current = hero.Health;
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