using Pact.Shared.State.Entities;

namespace Pact.Models;

public interface IToState<out T> where T : IEntity
{
    public T ToState();
}