using System;
using System.Collections.Generic;
using Godot;
using Pact.Client.UI;
using Pact.Shared.State.Entities;

namespace Pact.Client.Views;

[GlobalClass]
public partial class HeroView : View
{
    private static readonly PackedScene StatusIconScene = GD.Load<PackedScene>("uid://dstatus0icon2");

    private FacingDirection _facing;
    private bool _isCasting;

    private Vector2 _moveDelta;

    [Export] private FancyProgressBar _healthBar = null!;
    [Export] private FancyProgressBar _manaBar = null!;
    [Export] private Container _statusBar = null!;

    private readonly Dictionary<string, StatusIcon> _statusIcons = [];
    private readonly List<string> _staleKeys = [];
    
    [Export] public float IdleThreshold = 0.5f;
    [Export] public float RunThreshold = 2f;

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
        RunThreshold = RunThreshold.UnitsToPixels();
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
        
        if (Math.Abs(hero.MaxMana - _manaBar.Max) > 0.01f) _manaBar.Max = hero.MaxMana;
        if (Math.Abs(hero.Mana - _manaBar.Current) > 0.01f) _manaBar.Current = hero.Mana;

        UpdateStatuses(hero.Statuses);
    }

    private void UpdateStatuses(StatusState[] statuses)
    {
        statuses ??= [];
        foreach (var status in statuses)
        {
            if (!_statusIcons.TryGetValue(status.IconUid, out var icon))
            {
                icon = StatusIconScene.Instantiate<StatusIcon>();
                _statusBar.AddChild(icon);
                _statusIcons[status.IconUid] = icon;
            }
            icon.Bind(status);
        }

        _staleKeys.Clear();
        foreach (var (uid, _) in _statusIcons)
            if (!Array.Exists(statuses, s => s.IconUid == uid))
                _staleKeys.Add(uid);

        foreach (var uid in _staleKeys)
        {
            _statusIcons[uid].QueueFree();
            _statusIcons.Remove(uid);
        }
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