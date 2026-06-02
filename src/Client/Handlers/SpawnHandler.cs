using System;
using Pact.Models;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State.Entities;

namespace Pact.Client.Handlers;

public class SpawnHandler : MessageHandler<SpawnMessage>
{
    public override void Handle(SpawnMessage message)
    {
        switch (message.EntityType)
        {
            case EntityType.Hero:
                ClientRoot.Instance.SpawnView<HeroModel>(message.SpawnedEntityId, message.ModelId, message.Position);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}