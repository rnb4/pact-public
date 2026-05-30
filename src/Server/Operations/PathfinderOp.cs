using Godot;
using Pact.Shared.Components;
using Pact.Shared.State.Entities;

namespace Pact.Server.Operations;

public static class PathfinderOp
{
    public static void RequestPath<T>(ref T pathMover, Vector2 destination) where T : IEntity, IPathMove
    {
        var path = NavigationServer2D.MapGetPath(Game.Instance.NavMap, pathMover.Position, destination, true);
        pathMover.Path = path;
        pathMover.PathIndex = 0;
    }

    public static void Stop<T>(ref T pathMover) where T : IEntity, IPathMove
    {
        pathMover.Path = [];
        pathMover.PathIndex = 0;
        pathMover.MoveDirection = Vector2.Zero;
    }
}