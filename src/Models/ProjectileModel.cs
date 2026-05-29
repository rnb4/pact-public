using Godot;
using Pact.Models.Effects;
using Pact.Shared.State.Entities;

namespace Pact.Models;

[GlobalClass]
public partial class ProjectileModel : Model, IToState<HomingProjectileState>
{
    [Export] public PackedScene? ViewScene { get; set; }
    [Export] public float Speed { get; set; }
    [Export] public float Lifetime { get; set; }
    [Export] public float Radius { get; set; }
    [Export] public required EffectModel OnHitEffect { get; set; }

    public HomingProjectileState ToState() => new()
    {
        Radius = Radius.UnitsToPixels(),
        Speed = Speed.UnitsToPixels(),
        TimeRemaining = Lifetime,
    };
}
