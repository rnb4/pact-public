using MemoryPack;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class NotifyLocalIdMessage : IServerMessage
{
    public int LocalEntityId { get; set; }
}