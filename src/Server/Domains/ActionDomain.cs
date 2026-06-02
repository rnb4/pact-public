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

    public static bool CanMove(Entity casterId)
    {
        if (IsBasicAttackLocked(casterId)) return false;
        if (AbilityDomain.IsCasting(casterId)) return false;
        return true;
    }

    public static bool CanCast(Entity casterId, AbilitySlot slot)
    {
        if (AbilityDomain.IsOnCooldown(casterId, slot)) return false;
        if (AbilityDomain.IsCasting(casterId)) return false;
        return true;
    }

    public static bool CanCancelBasic(Entity casterId)
    {
        return AbilityDomain.Casts.Any(c => c.CasterId == casterId && c.Slot == AbilitySlot.Basic) &&
               !IsBasicAttackLocked(casterId);
    }

    public static bool IsBasicAttackLocked(Entity casterId)
    {
        foreach (var cast in AbilityDomain.Casts)
        {
            if (cast.CasterId != casterId) continue;
            if (cast.AbilityModel is not BasicAbilityModel basic) continue;
            var frame = AbilityDomain.CurrentFrame(cast);
            if (frame >= basic.PreCancelFrame && frame <= basic.PostCancelFrame) return true;
        }

        return false;
    }
}
