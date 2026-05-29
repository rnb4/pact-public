using Godot;

namespace Pact.Models;

[GlobalClass]
public partial class BasicAbilityModel : AbilityModel
{
    [Export] public int PreCancelFrame { get; set; }
    [Export] public int PostCancelFrame { get; set; }
}