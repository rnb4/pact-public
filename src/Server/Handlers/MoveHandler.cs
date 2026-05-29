using Pact.AutoLoad;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;

namespace Pact.Server.Handlers;

public class MoveHandler : IRequestHandler<MoveRequest>
{
    public Result Handle(MoveRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var hero) is { IsValid: false } casterResult)
            return casterResult;

        if (ActionDomain.CanCancelBasic(hero.EntityId))
        {
            AbilityDomain.CancelBasic(hero.EntityId);
            NetworkManager.Instance.BroadcastServerMessage(new CastCancelServerMessage { CasterId = hero.EntityId });
        }
        else if (!ActionDomain.CanMove(hero.EntityId))
        {
            ActionDomain.QueuedActions[hero.EntityId] = request;
            return Result.Ok;
        }
        
        PathfinderOp.RequestPath(ref hero, request.Destination);
        Game.Instance.State.Entities.Set(request.EntityId, hero);
        ActionDomain.QueuedActions.Remove(request.EntityId);
        return Result.Ok;
    }
}