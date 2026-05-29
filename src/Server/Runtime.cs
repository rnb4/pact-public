using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Pact.AutoLoad;
using Pact.Gameplay.Maps;
using Pact.Server.Handlers;
using Pact.Server.Operations;
using Pact.Server.Repos;
using Pact.Server.Systems;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.ServerMessages;
using Pact.Shared.State;

namespace Pact.Server;

public class Runtime
{
    public static Runtime Instance { get; private set; } = null!;
    public required Map Map { get; init; }
    public required Rid NavMap { get; init; }
    public required MultiplayerApi Multiplayer { get; init; }
    public readonly ConcurrentQueue<IRequest> IncomingRequests = [];
    
    // Repos
    public readonly GameState GameState = new();
    public readonly PlayerRepo PlayerRepo = new();
    public ModelRepo ModelRepo { get; } = ModelRepo.Build();
    
    public required IReadOnlyList<ISystem> TickOrder { get; init; }
    
    // Handlers
    private readonly AbilityHandler _abilityHandler = new();
    private readonly MoveHandler _moveHandler = new();
    private readonly CursorUpdateHandler _cursorUpdateHandler = new();


    private Runtime()
    {
        Instance = this;
    }
    
    public static Runtime Build(Map map, MultiplayerApi multiplayer)
    {
        return new Runtime
        {
            Map = map,
            TickOrder = [
                new HomingProjectileSystem(), 
                new MovementSystem(), 
                new AbilityCastSystem(), 
                new ActionQueueSystem(), 
                new CooldownSystem()
            ],
            Multiplayer = multiplayer,
            NavMap = map.NavigationRegion2D.GetNavigationMap(),
        };
    }
    
    public void Tick(double delta)
    {
        while (IncomingRequests.TryDequeue(out var request))
            Handle(request);
        
        GameState.Tick++;
        foreach (var system in TickOrder) system.Tick(delta);
        
        foreach (var peer in Multiplayer.GetPeers())
            NetworkManager.Instance.SendSnapshot(peer, new GameStateSnapshot
            {
                Heroes = GameState.Heroes().Select(e => e.Item2).ToArray(),
                Projectiles = GameState.HomingProjectiles().Select(e => e.Item2).ToArray(),
            });
    }
    
    public void Handle(IRequest request)
    {
        var result = request switch 
        {
            AbilityRequest r => _abilityHandler.Handle(r),
            MoveRequest r => _moveHandler.Handle(r),
            CursorUpdateRequest r => _cursorUpdateHandler.Handle(r),
            _ => Result.NotImplemented,
        };
        if (result.IsValid) return;
        GD.Print($"Request failed: {request}\nReason: {result}");
        NetworkManager.Instance.SendServerMessage(request.PeerId, new RejectionMessage(result.Reason));
    }
    
    private int _peerNum = 0;
    public void OnPeerConnected(long peerId)
    {
        var modelId = _peerNum++ % 2 == 0 ? "knight" : "wizard";
        var entityId = SpawnOp.SpawnHero(peerId, modelId, new Vector2(1000, 1000));
        GD.Print("OnPeerConnected: " + entityId);
        if (entityId == null) return;
        NetworkManager.Instance.SendServerMessage((int)peerId, new NotifyLocalIdMessage
        {
            LocalEntityId = entityId.Value,
        });
    }
    
    public void OnPeerDisconnected(long peerId)
    {
        
    }
}


