using Godot;
using Pact.Shared.Snapshot;

namespace Pact.Shared.State.Entities;

public interface IEntity
{
    public EntityId EntityId { get; set; }
    public string ModelId { get; set; }
    public string Name { get; set; }
    public Team Team { get; set; }
    public Vector2 Position { get; set; }
}