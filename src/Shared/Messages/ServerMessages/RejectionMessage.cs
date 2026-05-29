using MemoryPack;
using Pact.Shared.Messages.Results;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class RejectionMessage(RejectReason reason) : IServerMessage
{
    public readonly RejectReason Reason = reason;
}