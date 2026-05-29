using Pact.Shared.State.Entities;

namespace Pact.Models;

public interface IToState<out T> where T : Entity
{
    public T ToState();
}