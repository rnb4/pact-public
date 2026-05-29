using Godot;
using MemoryPack;
using Pact.Shared.Snapshot;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
[MemoryPackUnion(1, typeof(HeroState))]
[MemoryPackUnion(2, typeof(HomingProjectileState))]
public abstract partial record Entity
{
    public EntityId EntityId { get; set; }
    public string ModelId { get; set; } = "";
    public string Name { get; set; } = "";
    public Team Team { get; set; }
    public Vector2 Position { get; set; }
}