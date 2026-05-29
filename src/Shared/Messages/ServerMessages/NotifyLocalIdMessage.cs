using Godot;
using MemoryPack;
using Pact.AutoLoad;
using Pact.Client;
using Pact.Shared.Snapshot;

namespace Pact.Shared.ServerMessages;

[MemoryPackable]
public partial class NotifyLocalIdMessage : Messages.ServerMessages.IServerMessage
{
    public EntityId LocalEntityId { get; set; }
    
    public void Handle()
    {
        NetworkManager.Instance.LocalEntityId = LocalEntityId;
        GD.Print($"LocalEntityId: {NetworkManager.Instance.LocalEntityId}");
    }
}