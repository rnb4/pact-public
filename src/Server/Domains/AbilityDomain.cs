using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Pact.AutoLoad;
using Pact.Client;
using Pact.Models;
using Pact.Models.Triggers;
using Pact.Server.Contexts;
using Pact.Shared.Messages.ServerMessages;
using Pact.Shared.State;

namespace Pact.Server.Domains;

public static class AbilityDomain
{
    public static AbilityModel? GetAbility(HeroModel model, AbilitySlot slot)
    {
        if (slot == AbilitySlot.Basic) return model.BasicAttack;

        var heroAbility = slot switch
        {
            AbilitySlot.Primary => model.Primary,
            AbilitySlot.Secondary => model.Secondary,
            AbilitySlot.Tertiary => model.Tertiary,
            AbilitySlot.Ultimate => model.Ultimate,
            _ => null
        };
        return heroAbility?.Ranks is { Length: > 0 } ranks ? ranks[0] : null;
    }

    public static IEnumerable<(AbilityModel, AbilitySlot)> GetAbilities(HeroModel model)
    {
        if (model.PassiveAbility is { } passive) yield return (passive, AbilitySlot.Passive);
        if (model.BasicAttack is { } basic) yield return (basic, AbilitySlot.Basic);
        if (model.Primary?.Ranks is { Length: > 0 } primary) yield return (primary[0], AbilitySlot.Primary);
        if (model.Secondary?.Ranks is { Length: > 0 } secondary) yield return (secondary[0], AbilitySlot.Secondary);
        if (model.Tertiary?.Ranks is { Length: > 0 } tertiary) yield return (tertiary[0], AbilitySlot.Tertiary);
        if (model.Ultimate?.Ranks is { Length: > 0 } ultimate) yield return (ultimate[0], AbilitySlot.Ultimate);
    }
}