using Pact.Server.Contexts;
using Pact.Shared.Messages.Results;

namespace Pact.Server.Handlers;

public interface IRequestHandler<T> where T : class
{
    public Result Handle(T request);
}