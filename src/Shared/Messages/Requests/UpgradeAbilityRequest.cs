using MemoryPack;
using Pact.Shared.State;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class UpgradeAbilityRequest : IHeroRequest
{
    public AbilitySlot Slot { get; init; }
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
}