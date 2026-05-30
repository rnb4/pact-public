using Source = Pact.Server.Contexts.Source;

namespace Pact.Gameplay.Stats;
// TODO: data driven stats

public enum Operation
{
    Flat,
    PercentAdd,
    PercentMultiply
}

public record StatModifier(Operation Op, float Value, Source Source);