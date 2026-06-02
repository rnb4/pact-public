using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;

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
                case EntityTarget { Entity: var targetEntity }:
                {
                    if (!Game.World.IsAlive(targetEntity)) { casts.RemoveAt(i); continue; }
                    ref var health = ref Game.World.Get<Pool<Health>>(targetEntity);
                    if (health.Current <= 0) { casts.RemoveAt(i); continue; }
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    EffectOp.ApplyTrigger(cast.Trigger, cast.CasterId, targetEntity);
                    break;
                }
                case GroundTarget { Position: var position }:
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    EffectOp.ApplyTrigger(cast.Trigger, cast.CasterId, position);
                    break;
            }

            casts.RemoveAt(i);
        }
    }
}
