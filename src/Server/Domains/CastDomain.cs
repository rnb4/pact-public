using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Pact.AutoLoad;
using Pact.Client;
using Pact.Models;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Operations;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;

namespace Pact.Server.Domains;

public static class CastDomain
{
    private const double AnimationFps = 24.0;
    
    public static readonly List<Cast> Casts = [];

    public static void ScheduleCast(Context context, AbilitySlot slot, AbilityModel abilityModel)
    {
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel activate) continue;
            Casts.Add(new Cast(context, slot, abilityModel, activate, activate.FireFrame / AnimationFps));
        }
    }

    public static void Cast(Context context, AbilitySlot slot, AbilityModel abilityModel)
    {
        ref var lookDir = ref Game.Instance.World.Get<LookDirection>(context.Caster);
        var position = Game.Instance.World.Get<Position>(context.Caster);
        lookDir = new LookDirection(position.Value.DirectionTo(context.Target.Position));
        PathfinderOp.Stop(context.Caster);
        ScheduleCast(context, slot, abilityModel);
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel { AnimationName: not null } activate) continue;
            var message = new BeginCastServerMessage
                { CasterId = context.Caster.Id, AnimationName = activate.AnimationName, Facing = lookDir.Value.ToFacingDirection() };
            foreach (var peer in Game.Instance.Multiplayer.GetPeers())
                NetworkManager.Instance.SendServerMessage(peer, message);
        }
    }

    public static void CancelCast(Entity casterId, AbilitySlot slot)
    {
        Casts.RemoveAll(cast => cast.Context.Caster == casterId && cast.Slot == slot);
    }

    public static bool IsCasting(Entity caster)
    {
        return Casts.Any(cast => cast.Context.Caster == caster);
    }

    public static void CancelBasic(Entity casterId)
    {
        CancelCast(casterId, AbilitySlot.Basic);
    }

    public static int CurrentFrame(Cast pf)
    {
        return (int)((pf.Trigger.FireFrame / AnimationFps - pf.TimeRemaining) * AnimationFps);
    }
}

public record Cast(
    Context Context,
    AbilitySlot Slot,
    AbilityModel AbilityModel,
    ActivateTriggerModel Trigger,
    double TimeRemaining);
