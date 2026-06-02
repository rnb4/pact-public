namespace Pact.Shared.Messages.Requests;

public interface IHeroRequest : IRequest
{
    public int EntityId { get; init; }
}