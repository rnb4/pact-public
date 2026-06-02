using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Client.Views;

public partial class View : Node2D
{
    protected const float PositionSmoothing = 12f;
    protected Vector2 SmoothedPosition;
    protected AnimatedSprite2D Sprite = null!;
    public int Id { get; set; }


    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        if (!ClientRoot.Instance.GameState.TryGet<HomingProjectileState>(Id, out var entity)) return;
        GlobalPosition = entity.Position;
        SmoothedPosition = entity.Position;
    }
}