using MemoryPack;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class CastCancelServerMessage : IServerMessage
{
    public required int CasterId { get; init; }
}