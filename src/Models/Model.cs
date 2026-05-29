using Godot;

namespace Pact.Models;

[GlobalClass]
public abstract partial class Model : Resource, IModel
{
    public StringName ModelId { get; set; } = new();
}