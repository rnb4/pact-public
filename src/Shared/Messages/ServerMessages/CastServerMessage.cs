using MemoryPack;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class CastServerMessage : Messages.ServerMessages.IServerMessage
{
    public void Handle()
    {
        throw new System.NotImplementedException();
    }
}