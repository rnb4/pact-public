using Godot;
using Pact.Shared.Messages.ServerMessages;

namespace Pact.Client.Handlers;

public class AnnouncementHandler : MessageHandler<AnnouncementServerMessage>
{
    public override void Handle(AnnouncementServerMessage message)
    {
        GD.Print("SERVER ANNOUNCEMENT: {message.Content}");
    }
}