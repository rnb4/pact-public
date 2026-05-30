using MemoryPack;
using Pact.Client;
using EntityId = Pact.Shared.State.EntityId;

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