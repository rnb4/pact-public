using System.Collections.Generic;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;

namespace Pact.Client;

public class ClientGameState
{
    public readonly Dictionary<EntityId, HeroState> Heroes = [];
    public readonly Dictionary<EntityId, HomingProjectileState> Projectiles = [];
}