using Godot;
using Pact.AutoLoad;
using Pact.Gameplay.Maps;

namespace Pact.Server;

[GlobalClass]
public partial class ServerRoot : Node2D
{
    private Game _game = null!;

    public override void _Ready()
    {
        // Load map
        var map = GD.Load<PackedScene>("res://scenes/maps/map_01.tscn").Instantiate<Map>();
        AddChild(map);
        
        // Build runtime
        _game = Game.Build(map, Multiplayer);

        // Connect signals
        NetworkManager.Instance.RequestReceived += _game.IncomingRequests.Enqueue;
        NetworkManager.Instance.RequestUnreliableReceived += _game.Handle;
        Multiplayer.PeerConnected += _game.OnPeerConnected;
        Multiplayer.PeerDisconnected += _game.OnPeerDisconnected;
    }

    public override void _PhysicsProcess(double delta) =>_game.Tick(delta);
}