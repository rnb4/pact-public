using MemoryPack;

namespace Pact.Shared.State;

[MemoryPackable]
public partial struct GameStateSnapshot
{
    public uint Tick;
    public Entities.HeroState[] Heroes;
    public Entities.HomingProjectileState[] Projectiles;
}