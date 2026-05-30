using System;
using Source = Pact.Server.Contexts.Source;

namespace Pact.Gameplay.Stats;

// TODO: data driven stats
public class Stat
{
    public readonly ModifierSet Modifiers = new();
    private float _base;

    public float Base
    {
        get => _base;
        set
        {
            _base = value;
            Updated?.Invoke();
        }
    }

    public float Value => Modifiers.Apply(_base);

    public event Action? Updated;

    public void Add(StatModifier modifier)
    {
        Modifiers.Add(modifier);
        Updated?.Invoke();
    }

    public int RemoveBySource(Source source)
    {
        var removed = Modifiers.RemoveBySource(source);
        if (removed > 0) Updated?.Invoke();
        return removed;
    }
}