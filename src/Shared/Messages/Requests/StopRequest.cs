using MemoryPack;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
public partial class StopRequest : IHeroRequest
{
    public int PeerId { get; set; }
    public int EntityId { get; init; }
}