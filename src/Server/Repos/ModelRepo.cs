using System;
using System.Collections.Generic;
using Godot;
using Pact.Models;
using Pact.Models.Statuses;
using Model = Pact.Models.Model;

namespace Pact.Server.Repos;

public class ModelRepo
{
    private static readonly Dictionary<string, HeroModel> Heroes = [];
    private static readonly Dictionary<string, ProjectileModel> Projectiles = [];
    private static readonly Dictionary<string, StatusModel> Statuses = [];
    private static readonly Dictionary<Type, object> Lookup = new()
    {
        [typeof(HeroModel)] = Heroes,
        [typeof(ProjectileModel)] = Projectiles,
        [typeof(StatusModel)] = Statuses,
    };

    private void Register<T>(T model, Dictionary<string, T> registry) where T: Model
    {
        registry[model.Id] = model;
    }

    public bool TryGet<T>(string id, out T model) where T : Model
    {
        if (Lookup.TryGetValue(typeof(T), out var dict))
            return ((Dictionary<string, T>)dict).TryGetValue(id, out model!);
        model = null!;
        return false;
    }
    
    public static ModelRepo Build()
    {
        var registry = new ModelRepo();
        const string heroesPath = "res://resources/heroes/";
        foreach (var file in DirAccess.GetFilesAt(heroesPath))
            if ((file.EndsWith(".tres") || file.EndsWith(".res")) && GD.Load(heroesPath + file) is HeroModel model)
                registry.Register(model, Heroes);
        const string projectilesPath = "res://resources/projectiles/";
        foreach (var file in DirAccess.GetFilesAt(projectilesPath))
            if ((file.EndsWith(".tres") || file.EndsWith(".res")) && GD.Load(projectilesPath + file) is ProjectileModel model)
                registry.Register(model, Projectiles);
        return registry;
    }

    public void Bind(Runtime runtime) { }
}