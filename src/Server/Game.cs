using System.Collections.Concurrent;
using System.Collections.Generic;
using Arch.Core;
using Godot;
using Pact.AutoLoad;
using Pact.Server.Components;
using Pact.Server.Handlers;
using Pact.Server.Operations;
using Pact.Server.Repos;
using Pact.Server.Systems;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;
using Pact.Shared.State.Entities;
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
    // public GameState State = new();
    public readonly World World = World.Create();
    public readonly Dictionary<int, Entity> EntityById = [];
    public uint CurrentTick = 0;

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

        CurrentTick++;
        foreach (var system in TickOrder) system.Tick(delta);

        foreach (var peer in Multiplayer.GetPeers())
        {
            var heroes = new List<HeroState>();
            var heroQuery = new QueryDescription().WithAll<Hero, Position, Pool<Health>, Pool<Mana>, MetaData, LookDirection>();
            World.Query(in heroQuery, (Entity entity, ref Position position, ref Pool<Health> health, ref Pool<Mana> mana, ref MetaData meta, ref LookDirection lookDirection) =>
            {
                heroes.Add(new HeroState
                {
                    EntityId = entity.Id,
                    ModelId = meta.ModelId,
                    Name = meta.Name,
                    Position = position.Value,
                    Health = health.Current,
                    MaxHealth = health.Max,
                    Mana = mana.Current,
                    MaxMana = mana.Max,
                    LookDirection = lookDirection.Value,
                });
            });

            var projectiles = new List<HomingProjectileState>();
            var projectileQuery = new QueryDescription().WithAll<HomingProjectile, Position, MetaData>();
            World.Query(in projectileQuery, (Entity entity, ref Position position, ref MetaData metaData) =>
            {
                projectiles.Add(new HomingProjectileState
                {
                    Position = position.Value,
                    EntityId = entity.Id,
                    ModelId = metaData.ModelId,
                });
            });
            
            NetworkManager.Instance.SendSnapshot(peer, new GameStateSnapshot
            {
                Tick = CurrentTick,
                Heroes = heroes.ToArray(),
                HomingProjectiles = projectiles.ToArray(),
            });
        }
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
        var heroes = Models.Heroes;
        if (heroes.Count == 0) return;
        var model = heroes[_peerNum++ % heroes.Count];
        var hero = SpawnOp.SpawnHero(peerId, model, new Vector2(1000, 1000));
        GD.Print("OnPeerConnected: " + hero);
        NetworkManager.Instance.SendServerMessage((int)peerId, new NotifyLocalIdMessage
        {
            LocalEntityId = hero.Id
        });
    }

    public void OnPeerDisconnected(long peerId)
    {
    }
}