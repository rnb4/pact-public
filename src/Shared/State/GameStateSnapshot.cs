using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using Pact.Shared.State.Entities;

namespace Pact.Shared.State;

[MemoryPackable]
public partial struct GameStateSnapshot
{
    public uint Tick;
    public HeroState[] Heroes;
    public HomingProjectileState[] HomingProjectiles;
    
    [MemoryPackIgnore] public IEnumerable<IEntity> Entities => Heroes.Cast<IEntity>().Concat(HomingProjectiles.Cast<IEntity>());
}