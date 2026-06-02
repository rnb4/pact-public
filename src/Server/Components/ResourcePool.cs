using System;

namespace Pact.Server.Components;

public interface IPool;

public readonly struct Health : IPool;

public readonly struct Mana : IPool;

public record struct Pool<TKind>(float Max, float Current, float Regen) where TKind : IPool
{
    public readonly float Fraction => Max > 0f ? Current / Max : 0f;

    public float Replenish(float amount)
    {
        return amount < 0f ? 0f : Update(amount);
    }

    public float Deplete(float amount)
    {
        return amount < 0f ? 0f : -Update(-amount);
    }

    public float Regenerate() => Update(Regen);

    private float Update(float amount)
    {
        var before = Current;
        Current = Math.Clamp(Current + amount, 0f, Max);
        return Current - before;
    }
}