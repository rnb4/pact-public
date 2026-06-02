using MemoryPack;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class SellItemRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}