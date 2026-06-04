using System.Collections.Generic;
using Arch.Core;
using Pact.Shared.State;

namespace Pact.Server.Domains;

public static class CooldownDomain
{
    public static readonly Dictionary<(Entity, AbilitySlot), double> Cooldowns = [];

    public static void StartCooldown(Entity entity, AbilitySlot slot, double cooldown)
    {
        if (!Cooldowns.ContainsKey((entity, slot))) Cooldowns[(entity, slot)] = cooldown;
    }

    public static bool IsOnCooldown(Entity entity, AbilitySlot slot)
    {
        return Cooldowns.ContainsKey((entity, slot));
    }
}