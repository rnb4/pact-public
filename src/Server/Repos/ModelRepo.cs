using System;
using System.Collections.Generic;
using Godot;
using Pact.Models;
using Pact.Models.Statuses;
using Model = Pact.Models.Model;

namespace Pact.Server.Repos;

public class ModelRepo
{
    private readonly Dictionary<string, HeroModel> _heroes = [];
    private readonly Dictionary<Type, object> _lookup;
    private readonly Dictionary<string, ProjectileModel> _projectiles = [];
    private readonly Dictionary<string, StatusModel> _statuses = [];

    private ModelRepo()
    {
        _lookup = new Dictionary<Type, object>
        {
            [typeof(HeroModel)] = _heroes,
            [typeof(ProjectileModel)] = _projectiles,
            [typeof(StatusModel)] = _statuses
        };
    }

    public bool TryGet<T>(string id, out T model) where T : Model
    {
        if (_lookup.TryGetValue(typeof(T), out var dict))
            return ((Dictionary<string, T>)dict).TryGetValue(id, out model!);
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

    private void RegisterFolder<T>(string path, Dictionary<string, T> registry) where T : Model
    {
        foreach (var file in DirAccess.GetFilesAt(path))
            if ((file.EndsWith(".tres") || file.EndsWith(".res")) && GD.Load(path + file) is T model)
            {
                model.ModelId = file.ToLower().Replace(".tres", "").Replace(".res", "");
                Register(model, registry);
            }
    }

    private void Register<T>(T model, Dictionary<string, T> registry) where T : Model
    {
        registry[model.ModelId] = model;
    }
}