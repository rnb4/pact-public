using Godot;
using Pact.AutoLoad;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Client.Systems;

[GlobalClass]
public partial class MobaCamera : Camera2D
{
    private bool _lock;
    private bool _panning;
    [Export] public float PanSpeed { get; set; } = 9.5f.UnitsToPixels();
    [Export] public float EdgeThreshold { get; set; } = 0.1f;
    [Export] public float ZoomMin { get; set; } = 0.5f;
    [Export] public float ZoomMax { get; set; } = 2f;
    [Export] public float ZoomStep { get; set; } = 0.1f;

    public override void _Process(double delta)
    {
        var localId = NetworkManager.Instance.LocalEntityId;
        if (localId != EntityId.None)
        {
            if (Input.IsActionJustPressed("camera_lock")) _lock = !_lock;
            if (Input.IsActionJustReleased("camera_center")) _lock = false;
            if (_lock || Input.IsActionPressed("camera_center"))
                if (ClientRoot.Instance.GameState.Entities.TryGet(localId, out var hero))
                {
                    Position = hero.Position;
                    return;
                }
        }

        var viewport = GetViewport().GetVisibleRect();
        var mouse = GetViewport().GetMousePosition();
        var pan = Vector2.Zero;

        if (mouse.X < viewport.Size.X * EdgeThreshold) pan.X -= 1;
        else if (mouse.X > viewport.Size.X - viewport.Size.X * EdgeThreshold) pan.X += 1;

        if (mouse.Y < viewport.Size.Y * EdgeThreshold) pan.Y -= 1;
        else if (mouse.Y > viewport.Size.Y - viewport.Size.Y * EdgeThreshold) pan.Y += 1;

        if (pan != Vector2.Zero)
            Position += pan.Normalized() * PanSpeed * (float)delta / Zoom.X;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("camera_pan"))
        {
            _panning = true;
            GetViewport().SetInputAsHandled();
            return;
        }

        if (@event.IsActionReleased("camera_pan"))
        {
            _panning = false;
            GetViewport().SetInputAsHandled();
            return;
        }

        if (_panning && @event is InputEventMouseMotion motion)
        {
            Position -= motion.Relative / Zoom.X;
            _lock = false;
            GetViewport().SetInputAsHandled();
            return;
        }

        if (@event is not InputEventMouseButton mouse) return;
        if (!mouse.Pressed) return;

        var newZoom = Zoom.X;
        if (mouse.ButtonIndex == MouseButton.WheelUp) newZoom += ZoomStep;
        else if (mouse.ButtonIndex == MouseButton.WheelDown) newZoom -= ZoomStep;
        else return;

        newZoom = Mathf.Clamp(newZoom, ZoomMin, ZoomMax);
        Zoom = Vector2.One * newZoom;
        GetViewport().SetInputAsHandled();
    }
}