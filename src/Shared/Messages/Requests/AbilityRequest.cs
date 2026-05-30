using Godot;
using MemoryPack;
using Pact.Shared.State;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class AbilityRequest : IHeroRequest
{
    public AbilitySlot Slot { get; set; }
    public EntityId? TargetEntityId { get; set; }
    public Vector2? TargetPosition { get; set; }
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
}