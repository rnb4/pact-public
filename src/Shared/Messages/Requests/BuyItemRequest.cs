using MemoryPack;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class BuyItemRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public EntityId EntityId { get; init; }
}