using System;
using MemoryPack;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class CastServerMessage : IServerMessage
{
    public void Handle()
    {
        throw new NotImplementedException();
    }
}