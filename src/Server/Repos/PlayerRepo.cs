using System.Collections.Generic;
using Pact.Shared.Snapshot;

namespace Pact.Server.Repos;

public class PlayerRepo
{
    
    private readonly Dictionary<long, EntityId> _peerToEntity = [];
    
    public void Register(long peerId, EntityId entityId) => _peerToEntity.Add(peerId, entityId);

    public void Unregister(long peerId) => _peerToEntity.Remove(peerId);

    public bool Owns(long peerId, EntityId entityId) => _peerToEntity.ContainsKey(peerId) && _peerToEntity[peerId] == entityId;
}