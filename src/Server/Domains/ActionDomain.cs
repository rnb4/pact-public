using System.Collections.Generic;
using System.Linq;
using Godot;
using Pact.Gameplay.Stats;
using Pact.Models;
using Pact.Shared.ServerMessages;
using Pact.Shared.Snapshot;
using Pact.Shared.State;
using IRequest = Pact.Shared.Messages.Requests.IRequest;

namespace Pact.Server.Domains;

public static class ActionDomain
{
    public static readonly Dictionary<EntityId, IRequest> QueuedActions = [];

    public static bool CanMove(EntityId casterId)
    {
        if (IsBasicAttackLocked(casterId)) return false;
        if (AbilityDomain.IsCasting(casterId)) return false;
        return true;
    }

    public static bool CanCast(EntityId casterId, AbilitySlot slot)
    {
        if (AbilityDomain.IsOnCooldown(casterId, slot)) return false;
        if (AbilityDomain.IsCasting(casterId)) return false;
        return true;
    }

    public static bool CanCancelBasic(EntityId casterId) =>
        AbilityDomain.Casts.Any(c => c.CasterId == casterId && c.Slot == AbilitySlot.Basic) &&
        !IsBasicAttackLocked(casterId);

    public static bool IsBasicAttackLocked(EntityId casterId)
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
