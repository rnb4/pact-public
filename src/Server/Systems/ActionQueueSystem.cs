using System.Linq;
using Pact.Server.Domains;
using Pact.Shared.Messages.Requests;

namespace Pact.Server.Systems;

public class ActionQueueSystem : ISystem
{
    private static Runtime Runtime => Runtime.Instance;

    public void Tick(double delta)
    {
        foreach (var (id, request) in ActionDomain.QueuedActions.ToList())
        {
            if (request is MoveRequest moveRequest)
            {
                if (AbilityDomain.Casts.Any(c => c.CasterId == id)) continue;
                if (!ActionDomain.CanMove(id)) continue;
                ActionDomain.QueuedActions.Remove(id);
                Runtime.Handle(moveRequest);
            }
            else
            {
                Runtime.Handle(request);
            }
        }
    }
}
