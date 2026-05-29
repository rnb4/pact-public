using System;
using System.Collections.Generic;
using Godot;
using Pact.AutoLoad;
using Pact.Client.Handlers;
using Pact.Models;
using Pact.Client.Systems;
using Pact.Client.Views;
using Pact.Gameplay.Maps;
using Pact.Server.Repos;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.ServerMessages;
using Pact.Shared.Snapshot;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Client;

[GlobalClass]
public partial class ClientRoot : Node2D
{
    public static ClientRoot Instance { get; private set; } = null!;
    private static NetworkManager NetworkManager => NetworkManager.Instance;
    
    public readonly GameState GameState = new();
    private readonly ModelRepo _models = ModelRepo.Build();
    
    private readonly Dictionary<EntityId, View> _views = [];
    private readonly HashSet<EntityId> _knownHeroes = [];
    private readonly HashSet<EntityId> _knownProjectiles = [];
    
    private Node2D _viewsContainer = null!;
    private InputManager _input = null!;
    private MobaCamera _camera = null!;
    

    private readonly Dictionary<Type, IMessageHandler> _handlers = new()
    {
        [typeof(AnnouncementServerMessage)] = new AnnouncementHandler(),
        [typeof(BeginCastServerMessage)] = new BeginCastHandler(),
        [typeof(CastCancelServerMessage)] = new CastCancelHandler(),
        [typeof(NotifyLocalIdMessage)] = new NotifyLocalIdHandler(),
        [typeof(SpawnMessage)] = new SpawnHandler(),
    };

    public override void _Ready()
    {
        Instance = this;

        _input = new InputManager();
        AddChild(_input);
        
        var map = GD.Load<PackedScene>("res://scenes/maps/map_01.tscn").Instantiate<Map>();
        AddChild(map);
        
        _viewsContainer = new Node2D();
        _viewsContainer.YSortEnabled = true;
        AddChild(_viewsContainer);
        
        _camera = GetNode<MobaCamera>("%MobaCamera");
        
        NetworkManager.SnapshotReceived += ApplySnapshot;
        NetworkManager.ServerMessageReceived += ApplyServerMessage;
    }

    public void ApplySnapshot(GameStateSnapshot snapshot)
    {
        ApplyEntityGroup<HeroState, HeroModel>(snapshot.Heroes,  _knownHeroes);
        ApplyEntityGroup<HomingProjectileState, ProjectileModel>(snapshot.Projectiles, _knownProjectiles);
    }

    private void ApplyEntityGroup<TState, TModel>(
        IEnumerable<TState> incoming,
        HashSet<EntityId> known)
        where TState : Entity
        where TModel : Model, IHasView
    {
        var incomingIds = new HashSet<EntityId>();

        foreach (var state in incoming)
        {
            incomingIds.Add(state.EntityId);
            GameState.Entities.Sync(state.EntityId, state);
            if (!known.Contains(state.EntityId))
                SpawnView<TModel>(state.EntityId, state.ModelId);
        }
        
        foreach ( var id in known)
            if (!incomingIds.Contains(id))
            {
                DespawnView(id);
                GameState.Entities.Free(id);
            }
        
        known.Clear();
        known.UnionWith(incomingIds);
    }

    public void SpawnView<T>(EntityId id, string modelId) where T : Model, IHasView
    {
        _models.TryGet<T>(modelId, out var model);
        if (model.ViewScene == null) return;
        var view = model.ViewScene.Instantiate<View>();
        view.Id = id;
        _viewsContainer.AddChild(view);
        _views[id] = view;
    }

    public void DespawnView(EntityId id)
    {
        var view = _views[id];
        view.QueueFree();
        _views.Remove(id);
    }

    public bool TryGetView(EntityId id, out View? view) => _views.TryGetValue(id, out view);

    public void ApplyServerMessage(IServerMessage message) => _handlers[message.GetType()].Handle(message);
}