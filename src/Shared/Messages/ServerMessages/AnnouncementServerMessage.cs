using MemoryPack;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class AnnouncementServerMessage : IServerMessage
{
    public required string Content { get; set; }
}