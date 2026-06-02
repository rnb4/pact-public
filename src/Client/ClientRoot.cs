using System;
using System.Collections.Generic;
using Godot;
using Pact.AutoLoad;
using Pact.Client.Handlers;
using Pact.Client.Systems;
using Pact.Client.Views;
using Pact.Models;
using Pact.Server.Repos;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.Scenes;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Client;

[GlobalClass]
public partial class ClientRoot : Node2D
{
    private readonly Dictionary<Type, IMessageHandler> _handlers = new()
    {
        [typeof(AnnouncementServerMessage)] = new AnnouncementHandler(),
        [typeof(BeginCastServerMessage)] = new BeginCastHandler(),
        [typeof(CastCancelServerMessage)] = new CastCancelHandler(),
        [typeof(NotifyLocalIdMessage)] = new NotifyLocalIdHandler(),
        [typeof(RejectionMessage)] = new RejectionHandler(),
        [typeof(SpawnMessage)] = new SpawnHandler()
    };

    private readonly ModelRepo _models = ModelRepo.Build();

    private readonly Dictionary<int, View> _views = [];

    public readonly ClientGameState GameState = new();
    private MobaCamera _camera = null!;
    private InputManager _input = null!;

    private Node2D _viewsContainer = null!;
    public static ClientRoot Instance { get; private set; } = null!;
    private static NetworkManager NetworkManager => NetworkManager.Instance;

    public override void _Ready()
    {
        Instance = this;

        _input = new InputManager();
        AddChild(_input);

        var map = GD.Load<PackedScene>("res://scenes/maps/map_01.tscn").Instantiate<Map>();
        AddChild(map);

        _viewsContainer = new Node2D { YSortEnabled = true };
        AddChild(_viewsContainer);

        _camera = GetNode<MobaCamera>("%MobaCamera");

        NetworkManager.SnapshotReceived += ApplySnapshot;
        NetworkManager.ServerMessageReceived += ApplyServerMessage;
    }

    public void ApplySnapshot(GameStateSnapshot snapshot)
    {
        var (added, removed) = GameState.Apply(snapshot);
        foreach (var e in added)
        {
            switch (e)
            {
                case HeroState h: SpawnView<HeroModel>(h.EntityId, h.ModelId, e.Position); break;
                case HomingProjectileState p: SpawnView<ProjectileModel>(p.EntityId, p.ModelId, e.Position); break;
            }
        }
        foreach (var e in removed) DespawnView(e.EntityId);
    }

    public void SpawnView<T>(int id, long modelId, Vector2 position) where T : Model, IHasView
    {
        if (!_models.TryGet<T>(modelId, out var model) || model.ViewScene == null) return;
        var view = model.ViewScene.Instantiate<View>();
        view.Id = id;
        view.Position = position;
        _viewsContainer.AddChild(view);
        _views[id] = view;
    }

    public void DespawnView(int id)
    {
        var view = _views[id];
        view.QueueFree();
        _views.Remove(id);
    }

    public bool TryGetView(int id, out View? view)
    {
        return _views.TryGetValue(id, out view);
    }

    public void ApplyServerMessage(IServerMessage message)
    {
        _handlers[message.GetType()].Handle(message);
    }
}