using Godot;

namespace Pact.Models;

public partial class VfxModel : Resource
{
    public required PackedScene Scene { get; set; }
    public AudioStream? AudioStream { get; set; }
    public float PlaybackSpeed { get; set; } = 1f;
}