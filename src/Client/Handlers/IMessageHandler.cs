using Pact.Shared.Messages.ServerMessages;

namespace Pact.Client.Handlers;

public interface IMessageHandler
{
    void Handle(IServerMessage message);
}

public abstract class MessageHandler<T> : IMessageHandler where T : IServerMessage
{
    public void Handle(IServerMessage message)
    {
        Handle((T)message);
    }

    public abstract void Handle(T message);
}