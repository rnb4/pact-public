using System.Collections.Generic;
using Godot;

namespace Pact.Shared.Scenes;

public partial class Map : Node2D
{
    public readonly List<Vector2> BlueSpawns = [];
    public readonly List<Vector2> RedSpawns = [];
    public NavigationRegion2D NavigationRegion2D { get; private set; } = null!;

    public override void _Ready()
    {
        NavigationRegion2D = GetNodeOrNull<NavigationRegion2D>("Navigation/NavigationRegion (NR)");
        NavigationRegion2D?.BakeNavigationPolygon();

        PopulateSpawns(RedSpawns, "%RedSpawns");
        PopulateSpawns(BlueSpawns, "%BlueSpawns");
    }

    private void PopulateSpawns(List<Vector2> spawnList, NodePath nodePath)
    {
        foreach (var child in GetNode<Node2D>(nodePath).GetChildren())
            if (child is Marker2D marker)
                spawnList.Add(marker.Position);
    }
}