using Godot;

namespace Pact.Shared.Components;

public interface IPathMove
{
    public Vector2[] Path { get; set; }
    public int PathIndex { get; set; }
    public Vector2 MoveDirection { get; set; }
    public float Speed { get; set; }
    public Vector2 LookDirection { get; set; }
}