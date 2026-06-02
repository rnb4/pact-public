using System.Collections.Generic;
using Arch.Core;

namespace Pact.Server.Repos;

public class PlayerRepo
{
    private readonly Dictionary<long, Entity> _peerToEntity = []; // peerId -> Entity

    public void Register(long peerId, Entity entity)
    {
        _peerToEntity.Add(peerId, entity);
    }

    public void Unregister(long peerId)
    {
        _peerToEntity.Remove(peerId);
    }

    public bool TryGetEntity(long peerId, out Entity entity) => _peerToEntity.TryGetValue(peerId, out entity);

    public bool Owns(long peerId, Entity entity)
    {
        return _peerToEntity.ContainsKey(peerId) && _peerToEntity[peerId] == entity;
    }
}