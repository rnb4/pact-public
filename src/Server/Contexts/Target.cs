using Arch.Core;
using Godot;

namespace Pact.Server.Contexts;

public abstract record Target;

public record GroundTarget(Vector2 Position) : Target;

public record EntityTarget(Entity Entity) : Target;