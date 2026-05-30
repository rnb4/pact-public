using Godot;
using MemoryPack;
using Pact.AutoLoad;
using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class NotifyLocalIdMessage : IServerMessage
{
    public EntityId LocalEntityId { get; set; }

    public void Handle()
    {
        NetworkManager.Instance.LocalEntityId = LocalEntityId;
        GD.Print($"LocalEntityId: {NetworkManager.Instance.LocalEntityId}");
    }
}