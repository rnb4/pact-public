using MemoryPack;

namespace Pact.Shared.State.Entities;

[MemoryPackable]
public partial struct StatusState
{
    public string IconUid { get; set; }
    public string Name { get; set; }
    public float RemainingDuration { get; set; }
    public float Duration { get; set; }
    public int CurrentStacks { get; set; }
}
