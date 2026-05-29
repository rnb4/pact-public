using Godot;

namespace Pact.Models;

public interface IHasView
{
    public PackedScene? ViewScene { get; set; }
}