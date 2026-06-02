using Arch.Core;
using Godot;
using MemoryPack;
using Pact.Shared.Components;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial struct HeroState : IEntity
{
    public Vector2 MoveDirection { get; set; }
    public Vector2 LookDirection { get; set; }
    public float Speed { get; set; }

    // Resource Pools
    public float Health;
    public float MaxHealth;
    public float Mana;
    public float MaxMana;

    public bool IsAlive()
    {
        return Health > 0;
    }

    public int EntityId { get; set; }
    public long ModelId { get; set; }
    public string Name { get; set; }
    public Team Team { get; set; }
    public Vector2 Position { get; set; }
}