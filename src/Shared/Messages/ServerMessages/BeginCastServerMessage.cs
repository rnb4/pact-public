using MemoryPack;
using Pact.Client;
using Pact.Shared.ServerMessages;
using Pact.Shared.Snapshot;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class BeginCastServerMessage : IServerMessage
{
    public required EntityId CasterId { get; init; }
    public required string AnimationName { get; init; }
    public required FacingDirection Facing { get; init; }

    public void Handle()
    {
        if (!ClientRoot.Instance.TryGetView(CasterId, out var view)) return;
        view.PlayCastAnimation(AnimationName, Facing);
    }
}