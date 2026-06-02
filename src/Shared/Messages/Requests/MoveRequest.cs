using Godot;
using MemoryPack;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class MoveRequest : IHeroRequest
{
    public Vector2 Destination { get; init; }
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}