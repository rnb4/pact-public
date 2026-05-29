using Godot;
using Pact.Shared.Snapshot;

namespace Pact.Client.Views;

public partial class View : Node2D
{
    public EntityId Id { get; set; }
    
    protected const float PositionSmoothing = 12f;
    protected AnimatedSprite2D Sprite = null!;
    protected Vector2 SmoothedPosition;


    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        if (!ClientRoot.Instance.GameState.Entities.TryGet(Id, out var entity)) return;
        GlobalPosition = entity.Position;
        SmoothedPosition = entity.Position;
    }
}