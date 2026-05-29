using Pact.AutoLoad;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;

namespace Pact.Server.Systems;

public class AbilityCastSystem : ISystem
{
    private static Runtime Runtime => Runtime.Instance;

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
                    if (!Runtime.GameState.Heroes.TryGet(entity.EntityId, out var targetEntity) || !targetEntity.IsAlive())
                    {
                        casts.RemoveAt(i);
                        continue;
                    }
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    AbilityDomain.ApplyTrigger(cast.Trigger, ref targetEntity);
                    break;
                }
                case GroundTarget { Position: var position}:
                    AbilityDomain.StartCooldown(cast.CasterId, cast.Slot, cast.AbilityModel.Cooldown);
                    AbilityDomain.ApplyTrigger(cast.Trigger, position);
                    break;
            }

            casts.RemoveAt(i);
        }
    }
}
