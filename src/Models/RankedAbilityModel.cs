using Godot;

namespace Pact.Models;

[GlobalClass]
public partial class RankedAbilityModel : Resource
{
    [Export] public AbilityModel[] Ranks { get; set; } = [];
}