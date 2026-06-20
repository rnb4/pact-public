using System.Linq;
using Arch.Core;
using Godot;
using Pact.AutoLoad;
using Pact.Server.Components;
using Pact.Server.Operations;
using Pact.Shared.State.Entities;

namespace Pact.Server.Systems;

public class ResourceRegenSystem : ISystem
{
    public void Tick(double delta)
    {
        if (Game.Instance.CurrentTick % NetworkManager.TickRate != 0) return;
        var query = new QueryDescription().WithAny<Pool<Health>, Pool<Mana>>();
        Game.Instance.World.Query(in query, (ref Pool<Mana> mana, ref Pool<Health> health) =>
        {
            mana.Regenerate();
            health.Regenerate();
        });
    }
}