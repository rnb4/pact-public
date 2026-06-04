using Arch.Core;
using Pact.Models.Effects;

namespace Pact.Server.Components;

public record struct Hero(int Level);
public record struct Monster;
public record struct Minion;
public record struct Structure;
public record struct HomingProjectile(Entity Caster, Entity Target, EffectModel OnHitEffect, float Speed, float Radius, double TimeRemaining);
