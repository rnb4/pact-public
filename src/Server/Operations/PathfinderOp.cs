using Godot;
using Pact.Shared.Components;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;

namespace Pact.Server.Operations;

public static class PathfinderOp
{
    public static void RequestPath<T>(ref T pathMover, Vector2 destination) where T : Entity, IPathMove
    {
        var path = NavigationServer2D.MapGetPath(Runtime.Instance.NavMap, pathMover.Position, destination, optimize: true);
        pathMover.Path = path;
        pathMover.PathIndex = 0;
    }

    public static void Stop<T>(ref T pathMover) where T : Entity, IPathMove
    {
        pathMover.Path = [];
        pathMover.PathIndex = 0;
        pathMover.MoveDirection = Vector2.Zero;
    }
}