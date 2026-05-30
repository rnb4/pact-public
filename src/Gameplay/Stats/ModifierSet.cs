using System.Collections.Generic;
using System.Linq;
using Source = Pact.Server.Contexts.Source;

namespace Pact.Gameplay.Stats;

// TODO: data drive stats
public class ModifierSet
{
    private readonly List<StatModifier> _modifiers = [];

    public float Sum => _modifiers.Where(modifier => modifier.Op == Operation.Flat).Sum(modifier => modifier.Value);

    public float Multiplier
    {
        get
        {
            var pctAdd = 1f;
            var pctMul = 1f;
            foreach (var modifier in _modifiers)
                switch (modifier.Op)
                {
                    case Operation.PercentAdd:
                        pctAdd += modifier.Value;
                        break;
                    case Operation.PercentMultiply:
                        pctMul *= 1f * modifier.Value;
                        break;
                    case Operation.Flat:
                    default: continue;
                }

            return pctAdd * pctMul;
        }
    }

    public void Add(StatModifier modifier)
    {
        _modifiers.Add(modifier);
    }

    public int RemoveBySource(Source source)
    {
        var toRemove = _modifiers.Where(modifier => modifier.Source == source).ToList();
        foreach (var modifier in toRemove)
            _modifiers.Remove(modifier);
        return toRemove.Count;
    }

    public float Apply(float value)
    {
        return (value + Sum) * Multiplier;
    }
}