using Godot;

namespace Pact.Models;

[GlobalClass]
public abstract partial class Model : Resource, IModel
{
    public long ModelId { get; set; }
}