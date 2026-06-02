using System.Collections.Generic;
using Arch.Core;
using Pact.Server.Components;
using Pact.Server.Operations;

namespace Pact.Server.Systems;

public class HomingProjectileSystem : ISystem
{
    public void Tick(double delta)
    {
        var toDestroy = new List<Entity>();

        var query = new QueryDescription().WithAll<HomingProjectile, Position, MetaData, HomingData>();
        Game.Instance.World.Query(in query,
            (Entity entity, ref Position position, ref MetaData metaData, ref HomingData homing) =>
            {
                homing = homing with { TimeRemaining = homing.TimeRemaining - delta };
                if (homing.TimeRemaining <= 0) { toDestroy.Add(entity); return; }

                if (!Game.Instance.World.IsAlive(homing.TargetId)) { toDestroy.Add(entity); return; }
                ref var targetHealth = ref Game.Instance.World.Get<Pool<Health>>(homing.TargetId);
                if (targetHealth.Current <= 0) { toDestroy.Add(entity); return; }

                ref var targetPos = ref Game.Instance.World.Get<Position>(homing.TargetId);
                var toTarget = targetPos.Value - position.Value;
                var step = homing.Speed.UnitsToPixels() * (float)delta;
                var radiusPx = homing.Radius.UnitsToPixels();
                var distSq = toTarget.LengthSquared();

                if (distSq <= step * step && distSq <= radiusPx * radiusPx)
                {
                    EffectOp.ApplyOnHitEffect(metaData.ModelId, homing.CasterId, homing.TargetId);
                    toDestroy.Add(entity);
                    return;
                }

                position.Value += toTarget.Normalized() * step;
            });

        foreach (var e in toDestroy)
        {
            Game.Instance.EntityById.Remove(e.Id);
            Game.Instance.World.Destroy(e);
        }
    }
}
