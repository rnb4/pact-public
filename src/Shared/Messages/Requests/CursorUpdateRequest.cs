using Godot;
using MemoryPack;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class CursorUpdateRequest : IHeroRequest
{
    public Vector2 CursorPosition { get; init; }
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}