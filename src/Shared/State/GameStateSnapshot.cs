using MemoryPack;
using Pact.Shared.State.Entities;

namespace Pact.Shared.State;

[MemoryPackable]
public partial struct GameStateSnapshot
{
    public uint Tick;
    public HeroState[] Heroes;
    public HomingProjectileState[] Projectiles;
}