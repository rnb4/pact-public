using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;
using Pact.AutoLoad;
using Pact.Gameplay.Maps;
using Pact.Server.Handlers;
using Pact.Server.Repos;
using Pact.Server.Systems;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.ServerMessages;
using AbilityRequest = Pact.Shared.Messages.Requests.AbilityRequest;
using CursorUpdateRequest = Pact.Shared.Messages.Requests.CursorUpdateRequest;
using IRequest = Pact.Shared.Messages.Requests.IRequest;

namespace Pact.Server;

public class Runtime
{
    public static Runtime Instance { get; private set; } = null!;
    public required Map Map { get; init; }
    public required Rid NavMap { get; init; }
    public required MultiplayerApi Multiplayer { get; init; }
    public readonly ConcurrentQueue<IRequest> IncomingRequests = [];
    
    // Repos
    public readonly StateRepo StateRepo = new();
    public readonly PlayerRepo PlayerRepo = new();
    public ModelRepo ModelRepo { get; } = ModelRepo.Build();
    
    //Systems
    public required MovementSystem MovementSystem { get; init; }
    public required ActionQueueSystem ActionQueueSystem { get; init; }
    public required AbilityCastSystem AbilityCastSystem { get; init; }
    public required CooldownSystem CooldownSystem { get; init; }
    public required IReadOnlyList<ISystem> TickOrder { get; init; }
    
    // Handlers
    private readonly AbilityHandler _abilityHandler = new();
    private readonly MoveHandler _moveHandler = new();
    private readonly CursorUpdateHandler _cursorUpdateHandler = new();
    

    public Runtime()
    {
        Instance = this;
    }
    
    public void Tick(double delta)
    {
        while (IncomingRequests.TryDequeue(out var request))
            Handle(request);
        
        StateRepo.Tick++;
        foreach (var system in TickOrder) system.Tick(delta);
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
        GD.Print($"Failed to validate request: {request} reason: {result}");
        NetworkManager.Instance.SendServerMessage(request.PeerId, new RejectionMessage(result.Reason));
    }
    
    public static Runtime Build(Map map, MultiplayerApi multiplayer)
    {
        var movement = new MovementSystem();
        var actionQueue = new ActionQueueSystem();
        var abilityCast = new AbilityCastSystem();
        var cooldown = new CooldownSystem();

        var runtime = new Runtime
        {
            Map = map,
            TickOrder = [movement, abilityCast, actionQueue, cooldown],
            Multiplayer = multiplayer,
            NavMap = map.NavigationRegion2D.GetNavigationMap(),
            ActionQueueSystem = actionQueue,
            AbilityCastSystem = abilityCast,
            MovementSystem = movement,
            CooldownSystem = cooldown,
        };
        
        return runtime;
    }
}


