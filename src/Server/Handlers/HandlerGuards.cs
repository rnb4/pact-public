using Arch.Core;
using Pact.Models;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.State;

namespace Pact.Server.Handlers;

public static class HandlerGuards
{
    public static Result ValidateRequester(IHeroRequest request, out Entity caster)
    {
        if (!Game.Instance.Players.TryGetEntity(request.PeerId, out caster)) return Result.Unauthorized;
        if (caster.Id != request.EntityId) return Result.Unauthorized;
        ref var health = ref Game.Instance.World.Get<Pool<Health>>(caster);
        if (health.Current <= 0) return Result.IsDead;
        return Result.Ok;
    }

    public static Result ValidateTarget(CastRequest request, out Target target)
    {
        target = default;

        if (request.TargetEntityId is { } targetId)
        {
            if (!Game.Instance.EntityById.TryGetValue(targetId, out var targetEntity)) return Result.NotFound;
            if (!Game.Instance.World.IsAlive(targetEntity)) return Result.NotFound;
            ref var health = ref Game.Instance.World.Get<Pool<Health>>(targetEntity);
            if (health.Current <= 0) return Result.IsDead;
            target = targetEntity;
            return Result.Ok;
        }

        if (request.TargetPosition is { } position)
        {
            target = position;
            return Result.Ok;
        }

        return Result.InvalidTarget;
    }

    public static Result ValidateHeroModel(Entity caster, out HeroModel model)
    {
        var metaData = Game.Instance.World.Get<MetaData>(caster);
        if (!Game.Instance.Models.TryGet(metaData.ModelId, out model)) return Result.NotFound;
        return Result.Ok;
    }

    public static Result ValidateAbility(HeroModel heroModel, AbilitySlot slot, out AbilityModel abilityModel)
    {
        abilityModel = null!;
        var ability = AbilityDomain.GetAbility(heroModel, slot);
        if (ability is null) return Result.NotFound;
        abilityModel = ability;
        return Result.Ok;
    }
}
