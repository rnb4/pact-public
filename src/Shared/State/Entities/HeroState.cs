using Godot;
using MemoryPack;
using Pact.Shared.Components;
using Pact.Shared.Snapshot;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial struct HeroState : IEntity, IPathMove
{
    // Entity
    public EntityId EntityId { get; set; }
    public string ModelId { get; set; }
    public string Name { get; set; }
    public Team Team { get; set; }
    public Vector2 Position { get; set; }

    // PathMove
    public Vector2[] Path { get; set; }
    public int PathIndex { get; set; }
    public Vector2 MoveDirection { get; set; }
    public float Speed { get; set; }
    public Vector2 LookDirection { get; set; }
    
    // Resource Pools
    public float Health;
    public float MaxHealth;
    public float Mana;
    public float MaxMana;
    
    public bool IsAlive() => Health > 0;
}