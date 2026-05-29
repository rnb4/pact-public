using System;
using Godot;
using Pact.Server.Contexts;

namespace Pact.Models.Predicates;

public partial class HealthThresholdPredicate : Predicate
{
    [Export] public float Percent { get; set; }
    [Export] public bool IsAbove { get; set; }

    public sealed override required Func<Context, bool> Evaluate { get; init; }

    public HealthThresholdPredicate()
    {
        // Evaluate = (context) =>
        // {
            // var pools = context.Source.CasterId.GetComponent<ResourcePoolManager>();
            // if (pools == null) return false;
            // if (!pools.ResourcePools.TryGetValue(ResourceType.Health, out var health)) return false;
            // if (IsAbove) return health.Current >= Percent;
            // return health.Current <= Percent;
        // };
    }
}