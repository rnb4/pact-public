using System;
using MemoryPack;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class EndedServerMessage : IServerMessage
{
    public void Handle()
    {
        throw new NotImplementedException();
    }
}