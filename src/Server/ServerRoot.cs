using Godot;
using Pact.AutoLoad;
using Pact.Gameplay.Maps;

namespace Pact.Server;

[GlobalClass]
public partial class ServerRoot : Node2D
{
    private Runtime _runtime = null!;

    public override void _Ready()
    {
        // Load map
        var map = GD.Load<PackedScene>("res://scenes/maps/map_01.tscn").Instantiate<Map>();
        AddChild(map);
        
        // Build runtime
        _runtime = Runtime.Build(map, Multiplayer);

        // Connect signals
        NetworkManager.Instance.RequestReceived += _runtime.IncomingRequests.Enqueue;
        NetworkManager.Instance.RequestUnreliableReceived += _runtime.Handle;
        Multiplayer.PeerConnected += _runtime.OnPeerConnected;
        Multiplayer.PeerDisconnected += _runtime.OnPeerDisconnected;
    }

    public override void _PhysicsProcess(double delta) =>_runtime.Tick(delta);
}