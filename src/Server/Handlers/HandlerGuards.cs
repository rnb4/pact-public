using System;
using Pact.Models;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Repos;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Snapshot;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Server.Handlers;

public static class HandlerGuards
{
    public static Result ValidateRequester(IHeroRequest request, out HeroState caster)
    {
        caster = default;
        if (!Runtime.Instance.PlayerRepo.Owns(request.PeerId, request.EntityId)) return Result.Unauthorized;
        if (!Runtime.Instance.StateRepo.Heroes.TryGet(request.EntityId, out caster)) return Result.HeroNotFound;
        if (!caster.IsAlive()) return Result.IsDead;
        return Result.Ok;
    }

    public static Result ValidateTarget(AbilityRequest request, out Target target)
    {
        target = null!;
        
        if (request.TargetEntityId is { } targetId)
        {
            if (!Runtime.Instance.StateRepo.Heroes.TryGet(targetId, out var entity)) return Result.HeroNotFound;
            if (!entity.IsAlive()) return Result.IsDead;
            target = new EntityTarget(entity);
            return Result.Ok;
        }

        if (request.TargetPosition is { } position)
        {
            target = new GroundTarget(position);
            return Result.Ok;
        }
        
        return Result.InvalidTarget;
    }

    public static Result ValidateHeroModel(string id, out HeroModel model)
    {
        model = null!;
        if (!Runtime.Instance.ModelRepo.TryGet(id, out model)) return Result.HeroNotFound;
        return Result.Ok;
    }

    public static Result ValidateAbility(HeroModel heroModel, AbilitySlot slot, out AbilityModel abilityModel)
    {
        abilityModel = null!;
        var ability = AbilityDomain.GetAbility(heroModel, slot);
        if (ability is null) return Result.AbilityNotFound;
        abilityModel = ability;
        return Result.Ok;
    }
}