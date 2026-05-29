using System.Collections.Generic;
using System.Linq;
using Godot;
using Pact.AutoLoad;
using Pact.Client;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Models.Triggers;
using Pact.Server.Contexts;
using Pact.Server.Operations;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.Snapshot;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Server.Domains;

public static class AbilityDomain
{
    public const double AnimationFps = 24.0;

    public static readonly List<Cast> Casts = [];
    public static readonly Dictionary<(EntityId, AbilitySlot), double> Cooldowns = [];

    public static AbilityModel? GetAbility(HeroModel model, AbilitySlot slot)
    {
        if (slot == AbilitySlot.Basic) return model.BasicAttack;

        var heroAbility = slot switch
        {
            AbilitySlot.Primary => model.Primary,
            AbilitySlot.Secondary => model.Secondary,
            AbilitySlot.Tertiary => model.Tertiary,
            AbilitySlot.Ultimate => model.Ultimate,
            _ => null,
        };
        return heroAbility?.Ranks is { Length: > 0 } ranks ? ranks[0] : null;
    }

    public static int CurrentFrame(Cast pf) =>
        (int)((pf.Trigger.FireFrame / AnimationFps - pf.TimeRemaining) * AnimationFps);

    public static void StartCooldown(EntityId id, AbilitySlot slot, double cooldown)
    {
        if (!Cooldowns.ContainsKey((id, slot)))
            Cooldowns[(id, slot)] = cooldown;
    }
    
    public static bool IsOnCooldown(EntityId id, AbilitySlot slot) => Cooldowns.ContainsKey((id, slot));
    
    public static void ScheduleFire(EntityId casterId, AbilitySlot slot, Target target, AbilityModel abilityModel)
    {
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel activate) continue;
            Casts.Add(new Cast(casterId, target, slot, abilityModel, activate, activate.FireFrame / AnimationFps));
        }
    }
    
    public static void Cast(EntityId casterId, AbilitySlot slot, Target target, AbilityModel abilityModel, Runtime runtime, FacingDirection facing)
    {
        ScheduleFire(casterId, slot, target, abilityModel);
        foreach (var triggerModel in abilityModel.Triggers)
        {
            if (triggerModel is not ActivateTriggerModel { AnimationName: not null } activate) continue;
            var message = new BeginCastServerMessage { CasterId = casterId, AnimationName = activate.AnimationName, Facing = facing };
            foreach (var peer in runtime.Multiplayer.GetPeers())
                NetworkManager.Instance.SendServerMessage(peer, message);
        }
    }

    public static bool IsCasting(EntityId casterId) => Casts.Any(c => c.CasterId == casterId);
    
    public static void CancelBasic(EntityId casterId) => CancelCast(casterId, AbilitySlot.Basic);
    
    public static void CancelCast(EntityId casterId, AbilitySlot slot)
    {
        Casts.RemoveAll(c => c.CasterId == casterId && c.Slot == slot);
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, EntityId casterId, ref HeroState target)
    {
        switch (trigger.Effect)
        {
            case DamageEffect dmg:
                target.Health = System.Math.Max(0, target.Health - dmg.Amount);
                GD.Print($"{target.Name} took {dmg.Amount} damage");
                Runtime.Instance.GameState.Entities.Set(target.EntityId, target);
                break;
            case SpawnEffect spawn:
            {
                if (!Runtime.Instance.GameState.Entities.TryGet(casterId, out var caster)) return;
                SpawnOp.SpawnProjectile(spawn.Projectile.ModelId, caster.Position, target.EntityId);
                break;
            }
        }
    }

    public static void ApplyTrigger(ActivateTriggerModel trigger, EntityId casterId, Vector2 position)
    {
    }
}

public record Cast(EntityId CasterId, Target Target, AbilitySlot Slot, AbilityModel AbilityModel, ActivateTriggerModel Trigger, double TimeRemaining);
