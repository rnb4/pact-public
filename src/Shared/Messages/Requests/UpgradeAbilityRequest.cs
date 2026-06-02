using MemoryPack;
using Pact.Shared.State;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class UpgradeAbilityRequest : IHeroRequest
{
    public AbilitySlot Slot { get; init; }
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}