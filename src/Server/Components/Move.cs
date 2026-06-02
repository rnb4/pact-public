using Godot;

namespace Pact.Server.Components;

public record struct Move(Vector2 Direction, float Speed);