using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Pact.AutoLoad;
using Pact.Client;
using Pact.Models;
using Pact.Models.Triggers;
using Pact.Server.Contexts;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;

namespace Pact.Server.Domains;

public static class AbilityDomain
{
    public const double AnimationFps = 24.0;

    public static readonly List<Cast> Casts = [];
    public static readonly Dictionary<(Entity, AbilitySlot), double> Cooldowns = [];

    public static AbilityModel? GetAbility(HeroModel model, AbilitySlot slot)
    {
        if (slot == AbilitySlot.Basic) return model.BasicAttack;

        var heroAbility = slot switch
        {
            AbilitySlot.Primary => model.Primary,
            AbilitySlot.Secondary => model.Secondary,
            AbilitySlot.Tertiary => model.Tertiary,
            AbilitySlot.Ultimate => model.Ultimate,
            _ => null
        };
        return heroAbility?.Ranks is { Length: > 0 } ranks ? ranks[0] : null;
    }

    public static int CurrentFrame(Cast pf)
    {
        return (int)((pf.Trigger.FireFrame / AnimationFps - pf.TimeRemaining) * AnimationFps);
    }

    public static void StartCooldown(Entity id, AbilitySlot slot, double cooldown)
    {
        if (!Cooldowns.ContainsKey((id, slot)))
            Cooldowns[(id, slot)] = cooldown;
    }

    public static bool IsOnCooldown(Entity id, AbilitySlot slot)
    {
        return Cooldowns.ContainsKey((id, slot));
    }

    public static void ScheduleFire(Entity casterId, AbilitySlot slot, Target target, AbilityModel abilityModel)
    {
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel activate) continue;
            Casts.Add(new Cast(casterId, target, slot, abilityModel, activate, activate.FireFrame / AnimationFps));
        }
    }

    public static void Cast(Entity casterId, AbilitySlot slot, Target target, AbilityModel abilityModel, Game game,
        FacingDirection facing)
    {
        ScheduleFire(casterId, slot, target, abilityModel);
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel { AnimationName: not null } activate) continue;
            var message = new BeginCastServerMessage
                { CasterId = casterId.Id, AnimationName = activate.AnimationName, Facing = facing };
            foreach (var peer in game.Multiplayer.GetPeers())
                NetworkManager.Instance.SendServerMessage(peer, message);
        }
    }

    public static bool IsCasting(Entity casterId)
    {
        return Casts.Any(c => c.CasterId == casterId);
    }

    public static void CancelBasic(Entity casterId)
    {
        CancelCast(casterId, AbilitySlot.Basic);
    }

    public static void CancelCast(Entity casterId, AbilitySlot slot)
    {
        Casts.RemoveAll(c => c.CasterId == casterId && c.Slot == slot);
    }
}

public record Cast(
    Entity CasterId,
    Target Target,
    AbilitySlot Slot,
    AbilityModel AbilityModel,
    ActivateTriggerModel Trigger,
    double TimeRemaining);
