using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Pact.Models;
using Pact.Shared.Messages.Requests;
using Pact.Shared.State;

namespace Pact.Server.Domains;

public static class ActionDomain
{
    public static readonly Dictionary<Entity, IRequest> QueuedActions = [];

    public static bool CanMove(Entity caster)
    {
        if (IsBasicAttackLocked(caster)) return false;
        if (CastDomain.IsCasting(caster)) return false;
        return true;
    }

    public static bool CanCast(Entity caster, AbilitySlot slot)
    {
        if (CooldownDomain.IsOnCooldown(caster, slot)) return false;
        if (CastDomain.IsCasting(caster)) return false;
        return true;
    }

    public static bool CanCancelBasic(Entity caster)
    {
        return CastDomain.Casts.Any(c => c.Context.Caster == caster && c.Slot == AbilitySlot.Basic) &&
               !IsBasicAttackLocked(caster);
    }

    public static bool IsBasicAttackLocked(Entity caster)
    {
        foreach (var cast in CastDomain.Casts)
        {
            if (cast.Context.Caster != caster) continue;
            if (cast.AbilityModel is not BasicAbilityModel basic) continue;
            var frame = CastDomain.CurrentFrame(cast);
            if (frame >= basic.PreCancelFrame && frame <= basic.PostCancelFrame) return true;
        }

        return false;
    }
}
