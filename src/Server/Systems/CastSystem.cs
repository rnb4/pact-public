using System;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Results;

namespace Pact.Server.Systems;

public class CastSystem : ISystem
{
    private static Game Game => Game.Instance;

    public void Tick(double delta)
    {
        var casts = CastDomain.Casts;

        for (var i = casts.Count - 1; i >= 0; i--)
        {
            var cast = casts[i] with { TimeRemaining = casts[i].TimeRemaining - delta };
            casts[i] = cast;
            if (cast.TimeRemaining > 0) continue;
            CooldownDomain.StartCooldown(cast.Context.Caster, cast.Slot, cast.AbilityModel.Cooldown);
            TriggerOp.Fire<ActivateTriggerModel>(cast.AbilityModel, cast.Context);
            casts.RemoveAt(i);
        }
    }
}
