using Godot;
using Pact.Server.Repos;

namespace Pact.Server.Systems;

public class MovementSystem : ISystem
{
    private static GameState State => Game.Instance.State;
    
    // TODO: Add boidslike for avoidance

    public void Tick(double delta)
    {
        foreach (var (id, entity) in State.Heroes())
        {
            if (entity.Path.IsEmpty() || entity.PathIndex >= entity.Path.Length) continue;
            
            var next = entity.Path[entity.PathIndex];
            var toNext = next - entity.Position;
            var step = entity.Speed * (float)delta;

            var updated = entity;
            if (toNext.LengthSquared() <= step * step)
            {
                updated.Position = next;
                updated.PathIndex++;
                if (updated.PathIndex >= updated.Path.Length)
                    updated.MoveDirection = Vector2.Zero;
            }
            else
            {
                updated.Position += toNext.Normalized() * step;
                updated.MoveDirection = toNext.Normalized();
            }

            if (!Mathf.IsZeroApprox(updated.MoveDirection.LengthSquared()))
                updated.LookDirection = updated.MoveDirection;
            State.Entities.Set(id, updated);
        }
    }
}