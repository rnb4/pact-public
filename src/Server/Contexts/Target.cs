using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Server.Contexts;

public abstract partial record Target;
public record GroundTarget(Vector2 Position) : Target;
public record EntityTarget(IEntity Entity) : Target;
