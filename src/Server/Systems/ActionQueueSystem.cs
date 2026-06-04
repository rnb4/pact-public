using System.Linq;
using Pact.Server.Domains;
using Pact.Shared.Messages.Requests;

namespace Pact.Server.Systems;

public class ActionQueueSystem : ISystem
{
    private static Game Game => Game.Instance;

    public void Tick(double delta)
    {
        foreach (var (entity, request) in ActionDomain.QueuedActions.ToList())
            if (request is MoveRequest moveRequest)
            {
                if (CastDomain.Casts.Any(c => c.Context.Caster == entity)) continue;
                if (!ActionDomain.CanMove(entity)) continue;
                ActionDomain.QueuedActions.Remove(entity);
                Game.Handle(moveRequest);
            }
            else
            {
                Game.Handle(request);
            }
    }
}
