using Godot;
using Pact.AutoLoad;
using NotifyLocalIdMessage = Pact.Shared.Messages.ServerMessages.NotifyLocalIdMessage;

namespace Pact.Client.Handlers;

public class NotifyLocalIdHandler : MessageHandler<NotifyLocalIdMessage>
{
    public override void Handle(NotifyLocalIdMessage message)
    {
        NetworkManager.Instance.LocalEntityId = message.LocalEntityId;
        GD.Print($"LocalEntityId: {NetworkManager.Instance.LocalEntityId}");
    }
}