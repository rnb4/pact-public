using System;
using System.Collections.Generic;
using System.Linq;
using Pact.Shared.Snapshot;

namespace Pact.Shared.State;

public class EntityTable<T>
{
    private T[] _slots = new T[64];
    private byte[] _generation = new byte[64];
    private readonly HashSet<uint> _free = [];
    private uint _count;

    public EntityId Allocate(T initial)
    {
        if (_free.Count > 0)
        {
            var idx = _free.First();
            _free.Remove(idx);
            _slots[idx] = initial;
            return EntityId.New(idx, _generation[idx]);
        }
        else
        {
            var idx = _count++;
            EnsureCapacity(idx);
            _slots[idx] = initial;
            _generation[idx] = 1;
            return EntityId.New(idx, _generation[idx]);
        }
    }

    public void Sync(EntityId id, T value)
    {
        EnsureCapacity(id.Index);
        if (id.Index >= _count) _count = id.Index + 1;
        _generation[id.Index] = (byte)id.Generation;
        _slots[id.Index] = value;
        _free.Remove(id.Index);
    }

    public bool TryGet(EntityId id, out T value)
    {
        if (!IsAlive(id))
        {
            value = default!;
            return false;
        }
        value = _slots[id.Index];
        return true;
    }

    public void Set(EntityId id, T value)
    {
        if (IsAlive(id)) _slots[id.Index] = value;
    }

    public void BatchSet(IEnumerable<(EntityId, T)> set)
    {
        foreach (var tuple in set)
            Set(tuple.Item1, tuple.Item2);
    }

    public void Free(EntityId id)
    {
        if (!IsAlive(id)) return;
        _generation[id.Index]++;
        _free.Add(id.Index);
    }
    
    public bool IsAlive(EntityId id) => id.Index < _count && _generation[id.Index] == id.Generation;

    public IEnumerable<(EntityId id, T value)> All()
    {
        for (uint i = 0; i < _count; i++)
        {
            if (_free.Contains(i)) continue;
            var id = EntityId.New(i, _generation[i]);
            yield return (id, _slots[i]);
        }
    }

    private void EnsureCapacity(uint idx)
    {
        if (idx < _slots.Length) return;
        var newSize = Math.Max(_slots.Length * 2, (int)idx + 1);
        Array.Resize(ref _slots, newSize);
        Array.Resize(ref _generation, newSize);
    }
}