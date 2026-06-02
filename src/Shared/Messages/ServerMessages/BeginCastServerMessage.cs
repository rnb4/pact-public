using MemoryPack;
using Pact.Client;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class BeginCastServerMessage : IServerMessage
{
    public required int CasterId { get; init; }
    public required string AnimationName { get; init; }
    public required FacingDirection Facing { get; init; }
}