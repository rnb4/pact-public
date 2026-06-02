using Arch.Core;

namespace Pact.Server.Components;

public record struct Hero;
public record struct Monster;
public record struct Minion;
public record struct Structure;
public record struct HomingProjectile;

public record struct HomingData(Entity CasterId, Entity TargetId, float Speed, float Radius, double TimeRemaining);
