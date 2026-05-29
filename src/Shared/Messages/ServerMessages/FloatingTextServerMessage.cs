using MemoryPack;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class FloatingTextServerMessage : Messages.ServerMessages.IServerMessage
{
    public void Handle()
    {
        throw new System.NotImplementedException();
    }
}