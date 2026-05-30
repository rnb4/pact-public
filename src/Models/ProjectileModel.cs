using Godot;
using Pact.Models.Effects;
using Pact.Shared.State.Entities;

namespace Pact.Models;

[GlobalClass]
public partial class ProjectileModel : Model, IToState<HomingProjectileState>, IHasView
{
    [Export] public float Speed { get; set; }
    [Export] public float Lifetime { get; set; }
    [Export] public float Radius { get; set; }
    [Export] public required EffectModel OnHitEffect { get; set; }
    [Export] public PackedScene? ViewScene { get; set; }

    public HomingProjectileState ToState()
    {
        return new HomingProjectileState
        {
            Radius = Radius.UnitsToPixels(),
            Speed = Speed.UnitsToPixels(),
            TimeRemaining = Lifetime
        };
    }
}