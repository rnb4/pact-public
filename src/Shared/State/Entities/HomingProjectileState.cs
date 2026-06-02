using Godot;
using MemoryPack;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial struct HomingProjectileState : IEntity
{
    public int EntityId { get; set; }
    public long ModelId { get; set; }
    public Vector2 Position { get; set; }
}