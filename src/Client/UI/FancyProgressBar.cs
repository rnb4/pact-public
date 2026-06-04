using Godot;

namespace Pact.Client.UI;

[GlobalClass]
public partial class FancyProgressBar : Control
{
    private Color _actualColor = Colors.Green;
    private Color _recentColor = Colors.Red;
    [Export]
    public Color ActualColor
    {
        get => _actualColor;
        set
        {
            _actualColor = value;
            if (_actualBar?.GetThemeStylebox("fill") is StyleBoxFlat sb) sb.BgColor = value;
        }
    }

    [Export]
    public Color RecentColor
    {
        get => _recentColor;
        set
        {
            _recentColor = value;
            if (_recentBar?.GetThemeStylebox("fill") is StyleBoxFlat sb) sb.BgColor = value;
        }
    }
    
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
        _actualBar.AddThemeStyleboxOverride("fill", (StyleBox)_actualBar.GetThemeStylebox("fill").Duplicate());
        _recentBar.AddThemeStyleboxOverride("fill", (StyleBox)_recentBar.GetThemeStylebox("fill").Duplicate());
        _prev = Current;
        _recent = Current;
        ActualColor = _actualColor;
        RecentColor = _recentColor;
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