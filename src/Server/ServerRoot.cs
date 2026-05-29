using System.Linq;
using Godot;
using Pact.AutoLoad;
using Pact.Gameplay.Maps;
using Pact.Server.Operations;
using Pact.Shared.ServerMessages;
using GameStateSnapshot = Pact.Shared.State.GameStateSnapshot;

namespace Pact.Server;

[GlobalClass]
public partial class ServerRoot : Node2D
{
    public static ServerRoot Instance { get; private set; } = null!;

    private Runtime _runtime = null!;
    private int _peerNum = 0;

    public override void _Ready()
    {
        Instance = this;
        
        // Load map
        var map = GD.Load<PackedScene>("res://scenes/maps/map_01.tscn").Instantiate<Map>();
        AddChild(map);
        
        // Build runtime
        _runtime = Runtime.Build(map, Multiplayer);

        // Connect signals
        NetworkManager.Instance.RequestReceived += _runtime.IncomingRequests.Enqueue;
        NetworkManager.Instance.RequestUnreliableReceived += _runtime.Handle;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }

    public override void _PhysicsProcess(double delta)
    {
        _runtime.Tick(delta);
        
        foreach (var peer in Multiplayer.GetPeers())
            NetworkManager.Instance.SendSnapshot(peer, new GameStateSnapshot
            {
                Heroes = _runtime.StateRepo.Heroes.All().Select(e => e.value).ToArray(),
                Projectiles = _runtime.StateRepo.Projectiles.All().Select(e => e.value).ToArray(),
            });
    }

    private void OnPeerConnected(long peerId)
    {
        var modelId = _peerNum++ % 2 == 0 ? "knight" : "wizard";
        var entityId = SpawnOp.SpawnHero(peerId, modelId, new Vector2(1000, 1000));
        if (entityId == null) return;
        NetworkManager.Instance.SendServerMessage((int)peerId, new NotifyLocalIdMessage
        {
            LocalEntityId = entityId.Value,
        });
    }

    private void OnPeerDisconnected(long peerId)
    {
        
    }


}