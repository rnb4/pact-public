using Arch.Core;
using Godot;
using Pact.Server.Components;

namespace Pact.Server.Operations;

public static class PathfinderOp
{
    public static void RequestPath(Entity entity, Vector2 destination)
    {
        ref var position = ref Game.Instance.World.Get<Position>(entity);
        var path = NavigationServer2D.MapGetPath(Game.Instance.NavMap, position.Value, destination, true);
        ref var pathMove = ref Game.Instance.World.Get<PathMove>(entity);
        pathMove = pathMove with { Path = path, PathIndex = 0 };
    }

    public static void Stop(Entity entity)
    {
        ref var pathMove = ref Game.Instance.World.Get<PathMove>(entity);
        pathMove = pathMove with { Path = [], PathIndex = 0, MoveDirection = Vector2.Zero };
    }
}
