using System;
using Godot;
using MemoryPack;
using Pact.Client.Views;
using Pact.Shared.ServerMessages;
using Pact.Shared.Snapshot;
using GameStateSnapshot = Pact.Shared.State.GameStateSnapshot;
using IRequest = Pact.Shared.Messages.Requests.IRequest;
using IServerMessage = Pact.Shared.Messages.ServerMessages.IServerMessage;

namespace Pact.AutoLoad;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance { get; private set; } = null!;
    public const int ServerId = 1;
    public const int TickRate = 30;
    public View? LocalView { get; private set; }
    public EntityId LocalEntityId { get; set; } = EntityId.None;
    public Action<IRequest>? RequestReceived { get; set; }
    public Action<IRequest>? RequestUnreliableReceived { get; set; }
    public Action<IServerMessage>? ServerMessageReceived { get; set; }
    public Action<GameStateSnapshot>? SnapshotReceived { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void SendRequest(IRequest request) => 
        Send(ServerId, MethodName.ReceiveRequest, request);

    public void SendRequestUnreliable(IRequest request) => 
        Send(ServerId, MethodName.ReceiveRequestUnreliable, request);

    public void SendSnapshot(int peer, GameStateSnapshot snapshot) => 
        Send(peer, MethodName.ReceiveSnapshot, snapshot);

    public void SendServerMessage(int peer, IServerMessage serverMessage) => 
        Send(peer, MethodName.ReceiveServerMessage, serverMessage);

    public void BroadcastServerMessage(IServerMessage serverMessage)
    {
        foreach (var peer in Multiplayer.GetPeers())
            SendServerMessage(peer, serverMessage);
    }

    private static void Send<T>(int peer, StringName methodName, T packet) => 
        Instance.RpcId(peer, methodName,  MemoryPackSerializer.Serialize(packet));

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void ReceiveRequest(byte[] payload)
    {
        var senderId = Multiplayer.GetRemoteSenderId();
        var request = MemoryPackSerializer.Deserialize<IRequest>(payload);
        if (request == null) return;
        request.PeerId = senderId;
        RequestReceived?.Invoke(request);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void ReceiveRequestUnreliable(byte[] payload)
    {
        var senderId = Multiplayer.GetRemoteSenderId();
        var request = MemoryPackSerializer.Deserialize<IRequest>(payload);
        if (request == null) return;
        request.PeerId = senderId;
        RequestUnreliableReceived?.Invoke(request);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void ReceiveSnapshot(byte[] payload)
    {
        var snapshot = MemoryPackSerializer.Deserialize<GameStateSnapshot>(payload);
        SnapshotReceived?.Invoke(snapshot);
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void ReceiveServerMessage(byte[] payload)
    {
        var serverMessage = MemoryPackSerializer.Deserialize<IServerMessage>(payload);
        if (serverMessage == null) return;
        ServerMessageReceived?.Invoke(serverMessage);
    }
}