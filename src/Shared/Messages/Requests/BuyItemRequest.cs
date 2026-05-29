using MemoryPack;
using Pact.Shared.Snapshot;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class BuyItemRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
}