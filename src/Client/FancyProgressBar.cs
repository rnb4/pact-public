using Godot;

namespace Pact.Client;

[GlobalClass]
public partial class FancyProgressBar : Control
{
    private ProgressBar _recentBar = null!;
    private ProgressBar _actualBar = null!;

    private float _prev = -1f;
    private float _current;
    private float _recent;
    private float _drainTimer;

    private const float DrainDelay = 0.6f;
    private const float DrainSpeed = 0.25f;

    public float Current
    {
        get => _current;
        set => _current = Mathf.Min(value, Max);
    }
    public float Max { get; set; }

    public override void _Ready()
    {
        _recentBar = GetNode<ProgressBar>("%Recent");
        _actualBar = GetNode<ProgressBar>("%Actual");
        _prev = Current;
        _recent = Current;
    }

    public override void _Process(double delta)
    {
        _actualBar.Value = Current / Max * 100f;
        
        if (Current < _prev) _drainTimer = DrainDelay;
        else if (Current > _prev) _recent = Current;
        
        if (_drainTimer > 0f) _drainTimer -= (float)delta;
        else _recent = Mathf.MoveToward(_recent, Current, DrainSpeed * Max * (float)delta);

        _recentBar.Value = _recent / Max * 100f;
        _prev = Current;
    }
}