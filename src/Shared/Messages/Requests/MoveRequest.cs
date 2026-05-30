using Godot;
using MemoryPack;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class MoveRequest : IHeroRequest
{
    public Vector2 Destination { get; init; }
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
}