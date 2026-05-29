using Godot;
using MemoryPack;
using Pact.Client.Systems;
using Pact.Shared.Snapshot;
using ClientRoot = Pact.Client.ClientRoot;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class SpawnMessage : Messages.ServerMessages.IServerMessage
{
    public EntityType EntityType { get; set; }
    public EntityId SpawnedEntityId { get; set; }
    public required string ModelId { get; set; }
    public required Vector2 Position { get; set; }

    public void Handle()
    {
        GD.Print($"{ModelId}");
        ClientRoot.Instance.SpawnView(EntityType, SpawnedEntityId, ModelId);
    }
}