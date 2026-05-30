using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Pact.AutoLoad;
using Pact.Server.Handlers;
using Pact.Server.Operations;
using Pact.Server.Repos;
using Pact.Server.Systems;
using Pact.Shared;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;
using Map = Pact.Shared.Scenes.Map;
using NotifyLocalIdMessage = Pact.Shared.Messages.ServerMessages.NotifyLocalIdMessage;

namespace Pact.Server;

public class Game
{
    // Handlers
    private readonly AbilityHandler _abilityHandler = new();
    private readonly CursorUpdateHandler _cursorUpdateHandler = new();
    private readonly MoveHandler _moveHandler = new();
    public readonly ConcurrentQueue<IRequest> IncomingRequests = [];
    public readonly PlayerRepo Players = new();

    // Repos
    public GameState State = new();

    private int _peerNum;


    private Game()
    {
        Instance = this;
    }

    public static Game Instance { get; private set; } = null!;
    public required Map Map { get; init; }
    public required Rid NavMap { get; init; }
    public required MultiplayerApi Multiplayer { get; init; }
    public ModelRepo Models { get; } = ModelRepo.Build();

    public required IReadOnlyList<ISystem> TickOrder { get; init; }

    public static Game Build(Map map, MultiplayerApi multiplayer)
    {
        return new Game
        {
            Map = map,
            TickOrder =
            [
                new HomingProjectileSystem(),
                new MovementSystem(),
                new AbilityCastSystem(),
                new ActionQueueSystem(),
                new CooldownSystem()
            ],
            Multiplayer = multiplayer,
            NavMap = map.NavigationRegion2D.GetNavigationMap()
        };
    }

    public void Tick(double delta)
    {
        while (IncomingRequests.TryDequeue(out var request))
            Handle(request);

        State.Next();
        foreach (var system in TickOrder) system.Tick(delta);

        foreach (var peer in Multiplayer.GetPeers())
            NetworkManager.Instance.SendSnapshot(peer, new GameStateSnapshot
            {
                Heroes = State.Heroes().Select(e => e.Item2).ToArray(),
                Projectiles = State.HomingProjectiles().Select(e => e.Item2).ToArray()
            });
    }

    public void Handle(IRequest request)
    {
        var result = request switch
        {
            AbilityRequest r => _abilityHandler.Handle(r),
            MoveRequest r => _moveHandler.Handle(r),
            CursorUpdateRequest r => _cursorUpdateHandler.Handle(r),
            _ => Result.NotImplemented
        };
        if (result.IsValid) return;
        GD.Print($"Request failed: {request}\nReason: {result}");
        NetworkManager.Instance.SendServerMessage(request.PeerId, new RejectionMessage(result.Reason));
    }

    public void OnPeerConnected(long peerId)
    {
        var modelId = _peerNum++ % 2 == 0 ? "knight" : "wizard";
        var entityId = SpawnOp.SpawnHero(peerId, modelId, new Vector2(1000, 1000));
        GD.Print("OnPeerConnected: " + entityId);
        if (entityId == null) return;
        NetworkManager.Instance.SendServerMessage((int)peerId, new NotifyLocalIdMessage
        {
            LocalEntityId = entityId.Value
        });
    }

    public void OnPeerDisconnected(long peerId)
    {
    }
}