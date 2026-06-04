using Godot;

namespace Pact.Models.Effects;

[GlobalClass]
public partial class SpawnEffectModel : EffectModel
{
    [Export] public required ProjectileModel Projectile { get; set; }
}