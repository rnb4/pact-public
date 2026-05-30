using Godot;

namespace Pact.tools;

public partial class TileMapPostProcessor : Node
{
    public Node2D _PostProcess(Node2D baseNode)
    {
        var navigationRegion = baseNode.GetNodeOrNull<NavigationRegion2D>("Navigation/NavigationRegion (NR)");
        if (navigationRegion == null)
        {
            GD.PrintErr("Could not find navigation region");
            return baseNode;
        }
        
        var terrain = baseNode.GetNodeOrNull<Node2D>("Terrain");
        if (terrain == null)
        {
            GD.PrintErr("Could not find terrain");
            return baseNode;
        }

        terrain.Owner = null;
        terrain.Reparent(navigationRegion);
        terrain.Owner = navigationRegion.Owner;

        // var map = new Map();
        // map.AddChild(baseNode);
        
        return baseNode;
    }
}