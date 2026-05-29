using MemoryPack;
using Pact.Client;
using Pact.Client.Views;
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
        if (view is not HeroView heroView) return;
        heroView.PlayCastAnimation(AnimationName, Facing);
    }
}