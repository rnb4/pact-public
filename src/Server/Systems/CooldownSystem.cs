using System.Linq;
using Pact.Server.Domains;

namespace Pact.Server.Systems;

public class CooldownSystem : ISystem
{
    public void Tick(double delta)
    {
        foreach (var key in AbilityDomain.Cooldowns.Keys.ToList())
        {
            AbilityDomain.Cooldowns[key] -= delta;
            if (AbilityDomain.Cooldowns[key] <= 0)
                AbilityDomain.Cooldowns.Remove(key);
        }
    }
}