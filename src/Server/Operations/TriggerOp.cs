using System.Linq;
using Pact.Models;
using Pact.Models.Triggers;
using Pact.Server.Contexts;
using Pact.Shared.Messages.Results;

namespace Pact.Server.Operations;

public static class TriggerOp
{
    public static Result Fire<T>(AbilityModel abilityModel, Context context) where T : TriggerModel
    {
        foreach (var trigger in abilityModel.Triggers.Where(t=> t is T))
            if (EffectOp.Apply(trigger.Effect, context) is {} result && result != Result.Ok) return result;
        return Result.Ok;
    }
}