using System;
using Pact.Gameplay.Resources.Costs;

namespace Pact.Gameplay.Resources;

// TODO: data-drive resource pools
public class ResourcePool(ResourceType type, float max)
{
    private float _max;

    public ResourceType Type { get; } = type;
    public float Current { get; private set; }
    public float Max
    {
        get => _max;
        set
        {
            var diff = _max - Current;
            _max = value;
            Current += diff;
        }
    }
    public float Regen { get; set; } = 0f;
    public float RegenTickRate { get; set; } = 1f;
    public event Action? Depleted;
    public event Action<float>? Deducted;
    public event Action<float>? Restored;

    public bool CanAfford(float amount) => Current >= amount;
    
    public float Deduct(float amount)
    {
        var actual = Math.Min(amount, Current);
        Current -= actual;
        Deducted?.Invoke(actual);
        return actual;
    }

    public float Restore(float amount)
    {
        var actual = Math.Min(amount, Max - Current);
        Current += actual;
        Restored?.Invoke(actual);
        return actual;
    }

    public float Fill()
    {
        var diff = Max - Current;
        Current = Max;
        if (diff > 0) Restored?.Invoke(diff);
        return diff;
    }

    public void Tick(int tick)
    {
        if (Regen != 0f && tick % RegenTickRate == 0)
            Restore(Regen);
    }
}