using Godot;

namespace Pact.Models;

[GlobalClass]
public partial class ItemModel : Resource
{
    [Export] public int GoldCost { get; set; } = 1;
    [Export] public required AbilityModel Ability { get; set; }
}