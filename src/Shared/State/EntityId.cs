using MemoryPack;

namespace Pact.Shared.Snapshot;

[MemoryPackable]
public readonly partial record struct EntityId(uint Raw)
{
    public uint Index => Raw & 0x00FFFFFFu;
    public uint Generation => (Raw >> 24) & 0xFFu;

    public static EntityId New(uint index, uint generation)
        => new ((generation << 24) | (index & 0x00FFFFFFu));
    
    public static readonly EntityId None = default;
    
    public override string ToString() => $"#{Index}g{Generation}";
}