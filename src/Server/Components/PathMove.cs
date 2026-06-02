using Godot;

namespace Pact.Server.Components;

public record struct PathMove(float Speed, Vector2[] Path, int PathIndex, Vector2 MoveDirection);