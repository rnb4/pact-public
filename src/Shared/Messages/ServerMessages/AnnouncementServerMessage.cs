using MemoryPack;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class AnnouncementServerMessage : Messages.ServerMessages.IServerMessage
{
    public void Handle()
    {
        throw new System.NotImplementedException();
    }
}