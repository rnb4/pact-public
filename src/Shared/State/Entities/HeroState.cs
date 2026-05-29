using Godot;
using MemoryPack;
using Pact.Shared.Components;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial record HeroState : Entity, IPathMove
{
    // PathMove
    public Vector2[] Path { get; set; } = [];
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