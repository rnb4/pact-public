using Godot;
using MemoryPack;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial struct HomingProjectileState : IEntity
{
    public float Radius { get; set; }
    public float Speed { get; set; }
    public double TimeRemaining { get; set; }
    public EntityId TargetId { get; set; }

    public EntityId CasterId { get; set; }
    public EntityId EntityId { get; set; }
    public string ModelId { get; set; }
    public string Name { get; set; }
    public Team Team { get; set; }
    public Vector2 Position { get; set; }
}