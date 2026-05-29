using Godot;
using MemoryPack;
using Pact.Shared.Snapshot;
using Pact.Shared.State;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class AbilityRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
    public AbilitySlot Slot { get; set; }
    public EntityId? TargetEntityId { get; set; }
    public Vector2? TargetPosition { get; set; }
}