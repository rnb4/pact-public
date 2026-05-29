using Godot;
using MemoryPack;
using Pact.Shared.Snapshot;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial record HomingProjectileState : Entity
{
    public float Radius { get; set; }
    public float Speed { get; set; }
    public double TimeRemaining { get; set; }
    public EntityId TargetId { get; set; }
    public EntityId CasterId { get; set; }
}