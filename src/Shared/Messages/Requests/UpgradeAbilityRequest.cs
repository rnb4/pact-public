using MemoryPack;
using Pact.Shared.Snapshot;
using Pact.Shared.State;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class UpgradeAbilityRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
    public AbilitySlot Slot { get; init; }
}