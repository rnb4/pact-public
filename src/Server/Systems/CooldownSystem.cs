using System.Linq;
using Pact.Server.Domains;

namespace Pact.Server.Systems;

public class CooldownSystem : ISystem
{
    public void Tick(double delta)
    {
        foreach (var key in CooldownDomain.Cooldowns.Keys.ToList())
        {
            CooldownDomain.Cooldowns[key] -= delta;
            if (CooldownDomain.Cooldowns[key] <= 0)
                CooldownDomain.Cooldowns.Remove(key);
        }
    }
}