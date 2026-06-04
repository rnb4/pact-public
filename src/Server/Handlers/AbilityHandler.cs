using System.Diagnostics;
using System.Linq;
using Pact.Client;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.State;

namespace Pact.Server.Handlers;

public class AbilityHandler : IRequestHandler<CastRequest>
{
    private static readonly float RangeAdjustment = 2f.UnitsToPixels();
    private static readonly float RepathThreshold = 0.5f.UnitsToPixels();
    private static Game Game => Game.Instance;

    public Result Handle(CastRequest request)
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

        var range = abilityModel.Range.UnitsToPixels();
        var inRange = position.Value.DistanceTo(target.Position) <= range;

        if (inRange)
        {
            if (ActionDomain.CanCast(caster, request.Slot))
            {
                var context = new Context(caster, target, new Source(caster.Id, request.Slot, 0));
                CastDomain.Cast(context, request.Slot, abilityModel);
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
            var toTarget = position.Value.DirectionTo(target.Position);
            var stopPoint = target.Position - toTarget * (range - RangeAdjustment);

            if (!CastDomain.IsCasting(caster))
                if (target.IsEntity)
                {
                    ref var pathMove = ref Game.World.Get<PathMove>(caster);
                    var pathDone = pathMove.Path.Length == 0 || pathMove.PathIndex >= pathMove.Path.Length;
                    var needsPath = pathDone || pathMove.Path.Last().DistanceTo(stopPoint) >= RepathThreshold;
                    if (needsPath)
                        PathfinderOp.RequestPath(caster, stopPoint);
                }
                else
                {
                    PathfinderOp.RequestPath(caster, stopPoint);
                }

            ActionDomain.QueuedActions[caster] = request;
        }

        return Result.Ok;
    }
}
