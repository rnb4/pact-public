using System;
using Godot;
using MemoryPack;
using Pact.Models;
using Pact.Shared.Snapshot;
using ClientRoot = Pact.Client.ClientRoot;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class SpawnMessage : IServerMessage
{
    public EntityType EntityType { get; set; }
    public EntityId SpawnedEntityId { get; set; }
    public required string ModelId { get; set; }
    public required Vector2 Position { get; set; }

    public void Handle()
    {
        switch (EntityType)
        {
            case EntityType.Hero:
                ClientRoot.Instance.SpawnView<HeroModel>(SpawnedEntityId, ModelId);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}