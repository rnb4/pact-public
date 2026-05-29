using System.Collections.Generic;
using System.Linq;
using Godot;
using Pact.AutoLoad;
using Pact.Models;
using Pact.Client.Systems;
using Pact.Client.Views;
using Pact.Gameplay.Maps;
using Pact.Server.Repos;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;
using CursorUpdateRequest = Pact.Shared.Messages.Requests.CursorUpdateRequest;
using GameStateSnapshot = Pact.Shared.State.GameStateSnapshot;
using HeroState = Pact.Shared.State.Entities.HeroState;
using IServerMessage = Pact.Shared.Messages.ServerMessages.IServerMessage;

namespace Pact.Client;

[GlobalClass]
public partial class ClientRoot : Node2D
{
    public static ClientRoot Instance { get; private set; } = null!;
    
    private readonly Dictionary<EntityId, HomingProjectileState> _projectiles = [];
    private readonly Dictionary<EntityId, HeroState> _heroes = [];
    private readonly Dictionary<EntityId, HeroView> _views = [];
    private readonly HashSet<EntityId> _knownIds = [];
    
    public ModelRepo Models = ModelRepo.Build();
    private InputManager _input = null!;
    private Node2D _viewsContainer = null!;
    private MobaCamera _camera = null!;

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
        
        NetworkManager.Instance.SnapshotReceived += ApplySnapshot;
        NetworkManager.Instance.ServerMessageReceived += ApplyServerMessage;
    }

    public override void _PhysicsProcess(double delta)
    {
        var id = NetworkManager.Instance.LocalEntityId;
        if (id == EntityId.None) return;
        NetworkManager.Instance.SendRequestUnreliable(new CursorUpdateRequest
        {
            EntityId = id,
            CursorPosition = GetGlobalMousePosition()
        });
    }
    
    public IEnumerable<HeroState> Heroes => _heroes.Values;

    public IEnumerable<EntityId> AllTargetable() => _heroes.Select(keyValuePair => keyValuePair.Key);

    public bool TryGetHero(EntityId id, out HeroState hero) => _heroes.TryGetValue(id, out hero);
    public bool TryGetProjectile(EntityId id, out HomingProjectileState homingProjectile) => _projectiles.TryGetValue(id, out homingProjectile);
 
    public void SpawnView(EntityType entityType, EntityId id, string modelId)
    {
        if (entityType != EntityType.Hero) return;
        if (!Models.TryGet<HeroModel>(modelId, out var model) || model.ViewScene == null) return;
        var view = model.ViewScene.Instantiate<HeroView>();
        view.Id = id;
        _viewsContainer.AddChild(view);
        _views[id] = view;
    }

    public bool TryGetView(EntityId id, out HeroView? view) => _views.TryGetValue(id, out view);

    public void ApplySnapshot(GameStateSnapshot snapshot)
    {
        var incoming = new HashSet<EntityId>();

        foreach (var heroState in snapshot.Heroes)
        {
            incoming.Add(heroState.EntityId);
            _heroes[heroState.EntityId] = heroState;
            if (!_knownIds.Contains(heroState.EntityId))
                SpawnView(EntityType.Hero, heroState.EntityId, heroState.ModelId);
        }

        foreach (var id in _knownIds)
            if (!incoming.Contains(id))
                _heroes.Remove(id);

        _knownIds.Clear();
        _knownIds.UnionWith(incoming);
    }

    public void ApplyServerMessage(IServerMessage message) => message.Handle();
}