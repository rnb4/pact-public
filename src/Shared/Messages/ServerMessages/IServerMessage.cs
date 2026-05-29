using MemoryPack;
using Pact.Shared.ServerMessages;

namespace Pact.Shared.Messages.ServerMessages;

[MemoryPackable]
[MemoryPackUnion(0, typeof(VfxServerMessage))]
[MemoryPackUnion(1, typeof(FloatingTextServerMessage))]
[MemoryPackUnion(2, typeof(BeginCastServerMessage))]
[MemoryPackUnion(3, typeof(CastServerMessage))]
[MemoryPackUnion(4, typeof(AnnouncementServerMessage))]
[MemoryPackUnion(5, typeof(EndedServerMessage))]
[MemoryPackUnion(6, typeof(NotifyLocalIdMessage))]
[MemoryPackUnion(7, typeof(RejectionMessage))]
[MemoryPackUnion(8, typeof(SpawnMessage))]
[MemoryPackUnion(9, typeof(CastCancelServerMessage))]
public partial interface IServerMessage {}