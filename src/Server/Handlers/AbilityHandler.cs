using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Pact.Client;
using Pact.Server.Contexts;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Results;
using Pact.Shared.State;
using AbilityRequest = Pact.Shared.Messages.Requests.AbilityRequest;

namespace Pact.Server.Handlers;

public class AbilityHandler : IRequestHandler<AbilityRequest>
{
    private static Runtime Runtime => Runtime.Instance;
    private static readonly float RangeAdjustment = 2f.UnitsToPixels();
    private static readonly float RepathThreshold = 0.5f.UnitsToPixels();
    
    public Result Handle(AbilityRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var caster) is { IsValid: false } casterResult)
            return casterResult;
        if (HandlerGuards.ValidateHeroModel(caster.ModelId, out var casterModel) is { IsValid: false } heroResult)
            return heroResult;
        if (HandlerGuards.ValidateAbility(casterModel, request.Slot, out var abilityModel) is { IsValid: false } abilityResult)
            return abilityResult;
        if (HandlerGuards.ValidateTarget(request, out var target) is { IsValid: false} targetResult)
            return targetResult;
        
        var targetPos = target switch
        {
            EntityTarget { Entity: var e } => e.Position,
            GroundTarget { Position: var p } => p,
            _ => throw new UnreachableException(),
        };

        var range = abilityModel.Range.UnitsToPixels();
        var inRange = caster.Position.DistanceTo(targetPos) <= range;

        if (inRange)
        {
            if (ActionDomain.CanCast(request.EntityId, request.Slot))
            {
                caster.LookDirection = caster.Position.DirectionTo(targetPos);
                PathfinderOp.Stop(ref caster);
                Runtime.GameState.Heroes.Set(request.EntityId, caster);
                AbilityDomain.Cast(request.EntityId, request.Slot, target, abilityModel, Runtime, caster.LookDirection.ToFacingDirection());

                if (request.Slot == AbilitySlot.Basic)
                    ActionDomain.QueuedActions[request.EntityId] = request;
                else
                    ActionDomain.QueuedActions.Remove(request.EntityId);
            }
            else
            {
                ActionDomain.QueuedActions[request.EntityId] = request;
            }
        }
        else
        {
            var toTarget = caster.Position.DirectionTo(targetPos);
            var stopPoint = targetPos - toTarget * (range - RangeAdjustment);

            if (!AbilityDomain.IsCasting(request.EntityId))
            {
                switch (target)
                {
                    case EntityTarget:
                    {
                        var pathDone = caster.Path.IsEmpty() || caster.PathIndex >= caster.Path.Length;
                        var needsPath = pathDone || caster.Path.Last().DistanceTo(stopPoint) >= RepathThreshold;
                        if (needsPath)
                        {
                            PathfinderOp.RequestPath(ref caster, stopPoint);
                            Runtime.GameState.Heroes.Set(request.EntityId, caster);
                        }
                        break;
                    }
                    case GroundTarget:
                        PathfinderOp.RequestPath(ref caster, stopPoint);
                        Runtime.GameState.Heroes.Set(request.EntityId, caster);
                        break;
                }
            }
            ActionDomain.QueuedActions[request.EntityId] = request;
        }
        return Result.Ok;
    }
}