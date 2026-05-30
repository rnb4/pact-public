using MemoryPack;

namespace Pact.Shared.Messages.Requests;

[MemoryPackable]
[MemoryPackUnion(0, typeof(MoveRequest))]
[MemoryPackUnion(1, typeof(AbilityRequest))]
[MemoryPackUnion(2, typeof(BuyItemRequest))]
[MemoryPackUnion(3, typeof(SellItemRequest))]
[MemoryPackUnion(4, typeof(StopRequest))]
[MemoryPackUnion(5, typeof(CursorUpdateRequest))]
[MemoryPackUnion(6, typeof(UpgradeAbilityRequest))]
public partial interface IRequest
{
    int PeerId { get; set; }
}