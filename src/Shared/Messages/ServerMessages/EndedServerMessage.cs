using MemoryPack;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class EndedServerMessage : Messages.ServerMessages.IServerMessage
{
    public void Handle()
    {
        throw new System.NotImplementedException();
    }
}