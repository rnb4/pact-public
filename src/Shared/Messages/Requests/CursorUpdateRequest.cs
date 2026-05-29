using Godot;
using MemoryPack;
using Pact.Shared.Snapshot;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class CursorUpdateRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
    public Vector2 CursorPosition { get; init; }
}