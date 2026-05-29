using Godot;
using Pact.Shared.Snapshot;

namespace Pact.Client.Views;

public partial class View : Node2D
{
    public EntityId Id { get; set; }
    
    protected const float PositionSmoothing = 12f;
    protected AnimatedSprite2D Sprite = null!;
    protected Vector2 SmoothedPosition;
    protected Vector2 MoveDelta;

    public override void _Ready()
    {
        var material = new ShaderMaterial();
        material.Shader = GD.Load<Shader>("res://assets/shaders/outline.gdshader");
        Sprite.Material = material;
    }

    public void Highlight() => Sprite.Material.Set("shader_parameter/width", 1);
    public void UnHighlight() => Sprite.Material.Set("shader_parameter/width", 0);
}