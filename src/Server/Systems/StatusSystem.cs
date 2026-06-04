using Arch.Core;
using Pact.Server.Components;

namespace Pact.Server.Systems;

public class StatusSystem : ISystem
{
    public void Tick(double delta)
    {
        var query = new QueryDescription().WithAll<Statuses>();
        Game.Instance.World.Query(query, (Entity entity, ref Statuses statuses) =>
        {
            foreach (var status in statuses.Active)
            {
                status.RemainingDuration -= (float)delta;
                if (status.RemainingDuration >= 0) continue;
                
                
            }
        });
    }
}