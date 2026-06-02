using System.Diagnostics;
using System.Linq;
using Pact.Client;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Results;
using Pact.Shared.State;
using AbilityRequest = Pact.Shared.Messages.Requests.AbilityRequest;

namespace Pact.Server.Handlers;

public class AbilityHandler : IRequestHandler<AbilityRequest>
{
    private static readonly float RangeAdjustment = 2f.UnitsToPixels();
    private static readonly float RepathThreshold = 0.5f.UnitsToPixels();
    private static Game Game => Game.Instance;

    public Result Handle(AbilityRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var caster) is { IsValid: false } casterResult)
            return casterResult;
        if (HandlerGuards.ValidateHeroModel(caster, out var casterModel) is { IsValid: false } heroResult)
            return heroResult;
        if (HandlerGuards.ValidateAbility(casterModel, request.Slot, out var abilityModel) is
            { IsValid: false } abilityResult)
            return abilityResult;
        if (HandlerGuards.ValidateTarget(request, out var target) is { IsValid: false } targetResult)
            return targetResult;

        ref var position = ref Game.World.Get<Position>(caster);

        var targetPos = target switch
        {
            EntityTarget { Entity: var e } => Game.World.Get<Position>(e).Value,
            GroundTarget { Position: var p } => p,
            _ => throw new UnreachableException()
        };

        var range = abilityModel.Range.UnitsToPixels();
        var inRange = position.Value.DistanceTo(targetPos) <= range;

        if (inRange)
        {
            if (ActionDomain.CanCast(caster, request.Slot))
            {
                ref var lookDir = ref Game.World.Get<LookDirection>(caster);
                lookDir = new LookDirection(position.Value.DirectionTo(targetPos));
                PathfinderOp.Stop(caster);
                AbilityDomain.Cast(caster, request.Slot, target, abilityModel, Game,
                    lookDir.Value.ToFacingDirection());

                if (request.Slot == AbilitySlot.Basic)
                    ActionDomain.QueuedActions[caster] = request;
                else
                    ActionDomain.QueuedActions.Remove(caster);
            }
            else
            {
                ActionDomain.QueuedActions[caster] = request;
            }
        }
        else
        {
            var toTarget = position.Value.DirectionTo(targetPos);
            var stopPoint = targetPos - toTarget * (range - RangeAdjustment);

            if (!AbilityDomain.IsCasting(caster))
                switch (target)
                {
                    case EntityTarget:
                    {
                        ref var pathMove = ref Game.World.Get<PathMove>(caster);
                        var pathDone = pathMove.Path.Length == 0 || pathMove.PathIndex >= pathMove.Path.Length;
                        var needsPath = pathDone || pathMove.Path.Last().DistanceTo(stopPoint) >= RepathThreshold;
                        if (needsPath)
                            PathfinderOp.RequestPath(caster, stopPoint);
                        break;
                    }
                    case GroundTarget:
                        PathfinderOp.RequestPath(caster, stopPoint);
                        break;
                }

            ActionDomain.QueuedActions[caster] = request;
        }

        return Result.Ok;
    }
}
