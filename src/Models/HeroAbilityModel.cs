using Godot;

namespace Pact.Models;

[GlobalClass]
public partial class HeroAbilityModel : Resource
{
    [Export] public AbilityModel[] Ranks { get; set; } = [];
}