using Godot;
using MemoryPack;
using Pact.Shared.State;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class CastRequest : IHeroRequest
{
    public AbilitySlot Slot { get; set; }
    public int? TargetEntityId { get; set; }
    public Vector2? TargetPosition { get; set; }
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}