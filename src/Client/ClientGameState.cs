using System.Collections.Generic;
using System.Linq;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Client;

public class ClientGameState
{
    private readonly Dictionary<int, IEntity> _entities = [];

    public bool TryGet<T>(int entityId, out T entity) where T : struct, IEntity
    {
        if (_entities.TryGetValue(entityId, out var e) && e is T typed) { entity = typed; return true; }
        entity = default;
        return false;
    }
    
    public IEnumerable<IEntity> All() => _entities.Values;
    public IEnumerable<T> Query<T>() where T : struct, IEntity => _entities.Values.OfType<T>();

    public (IReadOnlyList<IEntity> Added, IReadOnlyList<IEntity> Removed) Apply(GameStateSnapshot snapshot)
    {
        var incoming = snapshot.Entities.ToDictionary(e => e.EntityId);
        var added = incoming.Values.Where(e => !_entities.ContainsKey(e.EntityId)).ToArray();
        var removed = _entities.Values.Where(e => !incoming.ContainsKey(e.EntityId)).ToArray();
        _entities.Clear();
        foreach (var (id, e) in incoming) _entities.Add(id, e);
        return (added, removed);
    }
}