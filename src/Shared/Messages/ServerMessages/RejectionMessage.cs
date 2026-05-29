using MemoryPack;
using Pact.Shared.Messages.Results;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class RejectionMessage(RejectReason reason) : Messages.ServerMessages.IServerMessage
{
    public RejectReason Reason = reason;
    
    public void Handle()
    {
    }
}