using System;
using System.Collections.Generic;
using Arch.Core;
using Godot;
using Pact.Server.Components;

namespace Pact.Server.Systems;

public class MovementSystem : ISystem
{
    // TODO: Add boidslike for avoidance

    public void Tick(double delta)
    {
        var query = new QueryDescription().WithAll<PathMove, Position, LookDirection>();
        Game.Instance.World.Query(in query, (ref Position position, ref PathMove pathMove, ref LookDirection lookDirection) =>
        {
            if (pathMove.Path.IsEmpty() || pathMove.PathIndex >= pathMove.Path.Length) return;
            
            var next = pathMove.Path[pathMove.PathIndex];
            var toNext = next - position.Value;
            var step = pathMove.Speed.UnitsToPixels() * (float)delta;

            if (toNext.LengthSquared() <= step * step)
            {
                position.Value = next;
                pathMove.PathIndex++;
                if (pathMove.PathIndex >= pathMove.Path.Length) pathMove.MoveDirection = Vector2.Zero;
            }
            else
            {
                position.Value += toNext.Normalized() * step;
                pathMove.MoveDirection = toNext.Normalized();
            }

            if (!Mathf.IsZeroApprox(pathMove.MoveDirection.LengthSquared()))
            {
                lookDirection.Value = pathMove.MoveDirection;
            }
        });
    }
}