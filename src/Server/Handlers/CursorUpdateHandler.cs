using Godot;
using Pact.Server.Components;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Messages.Results;

namespace Pact.Server.Handlers;

public class CursorUpdateHandler : IRequestHandler<CursorUpdateRequest>
{
    public Result Handle(CursorUpdateRequest request)
    {
        if (HandlerGuards.ValidateRequester(request, out var hero) is { IsValid: false } result)
            return result;

        ref var pathMove = ref Game.Instance.World.Get<PathMove>(hero);
        if (!Mathf.IsZeroApprox(pathMove.MoveDirection.LengthSquared())) return Result.Ok;

        ref var lookDir = ref Game.Instance.World.Get<LookDirection>(hero);
        ref var position = ref Game.Instance.World.Get<Position>(hero);
        lookDir = new LookDirection(position.Value.DirectionTo(request.CursorPosition));
        return Result.Ok;
    }
}
