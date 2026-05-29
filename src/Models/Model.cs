using Godot;

namespace Pact.Models;

[GlobalClass]
public abstract partial class Model : Resource, IModel
{
    [Export] public StringName Id { get; set; } = new();
}