using Arch.Core;
using Godot;
using MemoryPack;

namespace Pact.Shared.State.Entities;

public partial interface IEntity
{
    public int EntityId { get; set; }
    public long ModelId { get; set; }
    public Vector2 Position { get; set; }
}