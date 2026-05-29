using Godot;
using Pact.Shared.Messages.Results;
using CursorUpdateRequest = Pact.Shared.Messages.Requests.CursorUpdateRequest;

namespace Pact.Server.Handlers;

public class CursorUpdateHandler : IRequestHandler<CursorUpdateRequest>
{
    public Result Handle(CursorUpdateRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var hero) is { IsValid: false} result)
            return result;
        if (!Mathf.IsZeroApprox(hero.MoveDirection.LengthSquared())) return Result.Ok;
        hero.LookDirection = hero.Position.DirectionTo(request.CursorPosition);
        Runtime.Instance.GameState.Heroes.Set(hero.EntityId, hero);
        return Result.Ok;
    }
}