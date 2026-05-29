using Godot;
using Pact.AutoLoad;
using Pact.Serialization;

namespace Pact;

[GlobalClass]
public partial class Main : Node
{
    public override void _Ready()
    {
        MemoryPackRegistration.Register();

        Engine.PhysicsTicksPerSecond = NetworkManager.TickRate;

        if (OS.HasFeature("dedicated_server") || DisplayServer.GetName() == "headless")
            StartServer();
        else
            StartClient();
    }

    private void StartServer()
    {
        var peer = new ENetMultiplayerPeer();
        peer.CreateServer(4444, maxClients: 10);
        Multiplayer.MultiplayerPeer = peer;

        var host = GD.Load<PackedScene>("res://scenes/server_root.tscn").Instantiate();
        if (host == null)
        {
            GD.PrintErr("Failed to load server scene");
            return;
        }
        AddChild(host);
    }

    private void StartClient()
    {
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        var peer = new ENetMultiplayerPeer();
        peer.CreateClient("127.0.0.1", 4444);
        Multiplayer.MultiplayerPeer = peer;
    }
    
    private void OnConnectedToServer()
    {
        var client = GD.Load<PackedScene>("res://scenes/client_root.tscn").Instantiate();
        if (client == null)
        {
            GD.PrintErr("Could not load client scene");
            return;
        }
        AddChild(client);
    }
}