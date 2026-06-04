using Pact.AutoLoad;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;
using Pact.Shared.Messages.ServerMessages;

namespace Pact.Server.Handlers;

public class MoveHandler : IRequestHandler<MoveRequest>
{
    public Result Handle(MoveRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var caster) is { IsValid: false } casterResult)
            return casterResult;

        if (ActionDomain.CanCancelBasic(caster))
        {
            CastDomain.CancelBasic(caster);
            NetworkManager.Instance.BroadcastServerMessage(new CastCancelServerMessage { CasterId = caster.Id });
        }
        else if (!ActionDomain.CanMove(caster))
        {
            ActionDomain.QueuedActions[caster] = request;
            return Result.Ok;
        }

        PathfinderOp.RequestPath(caster, request.Destination);
        ActionDomain.QueuedActions.Remove(caster);
        return Result.Ok;
    }
}
