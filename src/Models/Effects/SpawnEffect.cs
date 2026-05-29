using Godot;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class SpawnEffect : EffectModel
{
    [Export] public required ProjectileModel Projectile { get; set; }
}