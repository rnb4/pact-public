using System.Collections.Generic;
using Arch.Core;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Operations;

namespace Pact.Server.Systems;

public class HomingProjectileSystem : ISystem
{
    public void Tick(double delta)
    {
        var toDestroy = new List<Entity>();

        var query = new QueryDescription().WithAll<HomingProjectile, Position, MetaData>();
        Game.Instance.World.Query(in query,
            (Entity entity, ref Position position, ref MetaData metaData, ref HomingProjectile homing) =>
            {
                homing = homing with { TimeRemaining = homing.TimeRemaining - delta };
                if (homing.TimeRemaining <= 0) { toDestroy.Add(entity); return; }

                if (!Game.Instance.World.IsAlive(homing.Target)) { toDestroy.Add(entity); return; }
                ref var targetHealth = ref Game.Instance.World.Get<Pool<Health>>(homing.Target);
                if (targetHealth.Current <= 0) { toDestroy.Add(entity); return; }

                ref var targetPos = ref Game.Instance.World.Get<Position>(homing.Target);
                var toTarget = targetPos.Value - position.Value;
                var step = homing.Speed.UnitsToPixels() * (float)delta;
                var radiusPx = homing.Radius.UnitsToPixels();
                var distSq = toTarget.LengthSquared();

                if (distSq <= step * step && distSq <= radiusPx * radiusPx)
                {
                    var source = new Source(homing.Caster.Id, 0, 0);
                    EffectOp.Apply(homing.OnHitEffect, new Context(homing.Caster, homing.Target, source));
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
