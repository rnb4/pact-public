using System;
using Godot;
using MemoryPack;
using Pact.Models;
using Pact.Shared.State.Entities;
using ClientRoot = Pact.Client.ClientRoot;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
public partial class SpawnMessage : IServerMessage
{
    public EntityType EntityType { get; set; }
    public int SpawnedEntityId { get; set; }
    public required long ModelId { get; set; }
    public required Vector2 Position { get; set; }
}