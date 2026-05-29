using System.Data.Common;
using System.Linq;
using Godot;
using Pact.Models;
using Pact.Server.Domains;
using Pact.Server.Operations;
using Pact.Shared.State;
using Pact.Shared.State.Entities;

namespace Pact.Server.Systems;

public class HomingProjectileSystem : ISystem
{
    private static EntityTable<Entity> Entities => Runtime.Instance.GameState.Entities;
    
    public void Tick(double delta)
    {
        foreach (var (id, e) in Entities.All())
        {
            if (e is not HomingProjectileState entity) continue;
            var projectile = entity;
            projectile.TimeRemaining -= delta;
            if (projectile.TimeRemaining <= 0) Entities.Free(id);
            if (!Entities.TryGet(projectile.TargetId, out var hero) 
                || hero is not HeroState target
                || !target.IsAlive())
            {
                Entities.Free(id);
                continue;
            }
            var toTarget = target.Position - projectile.Position;
            var step = projectile.Speed * (float)delta;
            var distSq = toTarget.LengthSquared();
            if (distSq <= step * step && distSq <= projectile.Radius * projectile.Radius)
            {
                EffectOp.ApplyEffect(projectile.ModelId, projectile.CasterId, ref target);
                Entities.Set(target.EntityId, target);
                Entities.Free(id);
                continue;
            }
            projectile.Position += toTarget.Normalized() * step;
            Entities.Set(id, projectile);
        }
    }
}