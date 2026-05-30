using MemoryPack;

namespace Pact.Shared.State;

[MemoryPackable]
public readonly partial record struct EntityId(uint Raw)
{
    public static readonly EntityId None = default;
    public uint Index => Raw & 0x00FFFFFFu;
    public uint Generation => (Raw >> 24) & 0xFFu;

    public static EntityId New(uint index, uint generation)
    {
        return new EntityId((generation << 24) | (index & 0x00FFFFFFu));
    }

    public override string ToString()
    {
        return $"#{Index}g{Generation}";
    }
}