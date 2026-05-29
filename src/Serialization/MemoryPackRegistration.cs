using Godot;
using MemoryPack;

namespace Pact.Serialization;

public static class MemoryPackRegistration
{
    public static void Register()
    {
        MemoryPackFormatterProvider.Register(new Vector2Formatter());
    }
}

public class Vector2Formatter : MemoryPackFormatter<Vector2>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Vector2 value)
    {
        writer.WriteUnmanaged(value.X, value.Y);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Vector2 value)
    {
        reader.ReadUnmanaged(out float x, out float y);
        value = new Vector2(x, y);
    }
}