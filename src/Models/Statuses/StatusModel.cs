using Godot;
using Pact.Gameplay.Statuses;

namespace Pact.Models.Statuses;

[GlobalClass]
public partial class StatusModel : Model
{
    [Export] public string Name { get; set; } = "Unnamed Status";
    [Export] public float Duration { get; set; } = 1f;
    [Export] public Effects.EffectModel[] ApplyEffects { get; set; } = [];
    [Export] public Effects.EffectModel[] RemoveEffects { get; set; } = [];
    [Export] public Triggers.TriggerModel[] Triggers { get; set; } = [];
    
    [ExportGroup("Stacks")]
    [Export] public float MaxStacks { get; set; } = 1f;
    [Export] public required StackTimeoutBehaviorModel StackTimeout { get; set; }
    [Export] public StackApplicationBehavior StackApplication { get; set; }
}