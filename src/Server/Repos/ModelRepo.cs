using System;
using System.Collections.Generic;
using Godot;
using Pact.Models;
using Pact.Models.Statuses;
using Model = Pact.Models.Model;

namespace Pact.Server.Repos;

public class ModelRepo
{
    private const long InvalidResourceUid = -1;
    private readonly Dictionary<long, HeroModel> _heroes = [];
    private readonly Dictionary<Type, object> _lookup;
    private readonly Dictionary<long, ProjectileModel> _projectiles = [];
    private readonly Dictionary<long, StatusModel> _statuses = [];

    public IReadOnlyList<HeroModel> Heroes => [.. _heroes.Values];

    private ModelRepo()
    {
        _lookup = new Dictionary<Type, object>
        {
            [typeof(HeroModel)] = _heroes,
            [typeof(ProjectileModel)] = _projectiles,
            [typeof(StatusModel)] = _statuses
        };
    }

    public bool TryGet<T>(long id, out T model) where T : Model
    {
        if (_lookup.TryGetValue(typeof(T), out var dict))
            return ((Dictionary<long, T>)dict).TryGetValue(id, out model!);
        model = null!;
        return false;
    }

    public static ModelRepo Build()
    {
        var repo = new ModelRepo();
        repo.RegisterFolder("res://resources/heroes/", repo._heroes);
        repo.RegisterFolder("res://resources/projectiles/", repo._projectiles);
        return repo;
    }

    private void RegisterFolder<T>(string path, Dictionary<long, T> registry) where T : Model
    {
        foreach (var file in DirAccess.GetFilesAt(path))
            if ((file.EndsWith(".tres") || file.EndsWith(".res")) && GD.Load(path + file) is T model)
            {
                var uid = ResourceLoader.GetResourceUid(path + file);
                if (uid == InvalidResourceUid) continue;
                model.ModelId = uid;
                Register(model, registry);
            }
    }

    private void Register<T>(T model, Dictionary<long, T> registry) where T : Model
    {
        registry[model.ModelId] = model;
    }
}
