using Arch.Core;
using Pact.Models.Statuses;
using Pact.Models.Triggers;
using Pact.Server.Components;
using Pact.Server.Contexts;
using Pact.Server.Operations;
using Pact.Shared.State;

namespace Pact.Server.Systems;

public class StatusSystem : ISystem
{
    public void Tick(double delta)
    {
        var query = new QueryDescription().WithAll<Statuses>();
        Game.Instance.World.Query(query, (Entity entity, ref Statuses statuses) =>
        {
            for (var i = statuses.Active.Count - 1; i >= 0; i--)
            {
                var status = statuses.Active[i];
                status.RemainingDuration -= (float)delta;
                if (status.RemainingDuration >= 0) continue;

                var context = new Context(entity, entity, new Source(entity.Id, AbilitySlot.None, 0));
                TriggerOp.Fire<ActivateTriggerModel>(status.Model.Triggers, context);

                if (status.StackTimeout is RemoveNStackTimeoutBehavior removeN)
                {
                    status.CurrentStacks -= removeN.Amount;
                    if (status.CurrentStacks > 0)
                    {
                        status.RemainingDuration = status.Model.Duration;
                        continue;
                    }
                }

                TriggerOp.Fire<OnRemoveTriggerModel>(status.Model.Triggers, context);
                statuses.Active.RemoveAt(i);
            }
        });
    }
}
