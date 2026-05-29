using Godot;
using Pact.Shared.Snapshot;

namespace Pact.Client.Views;

[GlobalClass]
public partial class ProjectileView : View
{
    
    public override void _Process(double delta)
    {
       if (!ClientRoot.Instance.GameState.Projectiles.TryGetValue(Id, out var entity)) return;
       
    }
}