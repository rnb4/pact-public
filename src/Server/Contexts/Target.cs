using Arch.Core;
using Godot;
using Pact.Server.Components;

namespace Pact.Server.Contexts;

public readonly record struct Target
{
    public readonly bool IsEntity;
    public readonly Entity Entity;
    private readonly Vector2 _position;

    public Vector2 Position => IsEntity
        ? Game.Instance.World.Get<Position>(Entity).Value
        : _position;

    private Target(Entity e) { IsEntity = true; Entity = e; }
    private Target(Vector2 pos) { IsEntity = false; _position = pos; }

    public static implicit operator Target(Entity e) => new(e);
    public static implicit operator Target(Vector2 p) => new(p);
}
