using Pact.AutoLoad;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.State.Entities;

namespace Pact.Server.Systems;

public class AbilityCastSystem : ISystem
{
    private static Game Game => Game.Instance;

    public void Tick(double delta)
    {
        var casts = AbilityDomain.Casts;

        for (var i = casts.Count - 1; i >= 0; i--)
        {
            var cast = casts[i] with { TimeRemaining = casts[i].TimeRemaining - delta };
            casts[i] = cast;
            if (cast.TimeRemaining > 0) continue;

            switch (cast.Target)
            {
                case EntityTarget { Entity: var entity}:
                {
                    if (!Game.State.Entities.TryGet(entity.EntityId, out var targetEntity) 
                        || targetEntity is not HeroState hero 
                        || !hero.IsAlive())
                    {
                        casts.RemoveAt(i);
                        continue;
                    }
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    EffectOp.ApplyTrigger(cast.Trigger, cast.CasterId, ref hero);
                    break;
                }
                case GroundTarget { Position: var position}:
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    EffectOp.ApplyTrigger(cast.Trigger, cast.CasterId, position);
                    break;
            }

            casts.RemoveAt(i);
        }
    }
}
