using Pact.Client.Views;
using Pact.Shared.Messages.ServerMessages;

namespace Pact.Client.Handlers;

public class BeginCastHandler : MessageHandler<BeginCastServerMessage>
{
    public override void Handle(BeginCastServerMessage message)
    {
        if (!ClientRoot.Instance.TryGetView(message.CasterId, out var view)) return;
        if (view is not HeroView heroView) return;
        heroView.PlayCastAnimation(message.AnimationName, message.Facing);
    }
}