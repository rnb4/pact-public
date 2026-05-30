using System.Linq;
using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Server.Systems;

public class ResourceRegenSystem : ISystem
{
    public void Tick(double delta)
    {
        foreach (var (id, entity) in Game.Instance.State.Entities.All())
        {
            if (entity is not HeroState hero) continue;
            
            // regenerate health via op
            
            // regenerate mana via op
        }
    }

}