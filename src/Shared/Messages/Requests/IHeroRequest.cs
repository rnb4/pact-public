using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Shared.Messages.Requests;

public interface IHeroRequest : IRequest
{
    public EntityId EntityId { get; init; }
}