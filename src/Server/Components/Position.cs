using Godot;

namespace Pact.Server.Components;

public record struct Position(Vector2 Value);
public record struct Velocity(Vector2 Value);
public record struct LookDirection(Vector2 Value);

public record struct Homing(Vector2 Position);