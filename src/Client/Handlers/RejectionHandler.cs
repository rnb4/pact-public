using Godot;
using RejectionMessage = Pact.Shared.Messages.ServerMessages.RejectionMessage;

namespace Pact.Client.Handlers;

public class RejectionHandler : MessageHandler<RejectionMessage>
{
    public override void Handle(RejectionMessage message)
    {
        // TODO: make these not bad
        GD.Print($"Failed to validate request: reason = {message.Reason}");
    }
}