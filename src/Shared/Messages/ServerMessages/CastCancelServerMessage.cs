using MemoryPack;
using Pact.Client;
using Pact.Shared.ServerMessages;
using Pact.Shared.Snapshot;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class CastCancelServerMessage : IServerMessage
{
    public required EntityId CasterId { get; init; }
    
    public void Handle()
    {
        if (!ClientRoot.Instance.TryGetView(CasterId, out var view)) return;
    }
}