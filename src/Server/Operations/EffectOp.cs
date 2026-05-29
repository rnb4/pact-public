using Godot;
using Pact.Models;
using Pact.Models.Effects;
using Pact.Shared.Snapshot;
using Pact.Shared.State.Entities;

namespace Pact.Server.Operations;

public static class EffectOp
{
    public static void ApplyEffect(string modelId, EntityId casterId, ref HeroState target)
    {
        if (!Runtime.Instance.ModelRepo.TryGet<ProjectileModel>(modelId, out var model)) return;
        switch (model.OnHitEffect)
        {
            case DamageEffect dmg:
                target.Health = System.Math.Max(0, target.Health - dmg.Amount);
                GD.Print($"{target.Name} took {dmg.Amount} damage");
                Runtime.Instance.GameState.Entities.Set(target.EntityId, target);
                break;
            case SpawnEffect spawn:
            {
                if (!Runtime.Instance.GameState.Entities.TryGet(casterId, out var caster)) return;
                SpawnOp.SpawnProjectile(spawn.Projectile.ModelId, caster.Position, target.EntityId);
                break;
            }
        }
    }
}