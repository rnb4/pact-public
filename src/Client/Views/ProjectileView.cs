using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Client.Views;

[GlobalClass]
public partial class ProjectileView : View
{
    public override void _Ready()
    {
        base._Ready();
        Sprite.Play("default");
    }

    public override void _Process(double delta)
    {
        if (!ClientRoot.Instance.GameState.TryGet<HomingProjectileState>(Id, out var entity)) return;
        SmoothedPosition = SmoothedPosition.Lerp(entity.Position, PositionSmoothing * (float)delta);
        var moveDelta = SmoothedPosition - GlobalPosition;
        GlobalPosition = SmoothedPosition;
        LookAt(GlobalPosition + moveDelta);
    }
}