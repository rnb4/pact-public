using Pact.Shared.Snapshot;

namespace Pact.Shared.Messages.Requests;

public interface IHeroRequest : IRequest
{
    public EntityId EntityId { get; init; }
}