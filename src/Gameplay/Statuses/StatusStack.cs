using Pact.Server.Contexts;
using Source = Pact.Server.Contexts.Source;

namespace Pact.Gameplay.Statuses;
// TODO: data driven statuses

public record StatusStack(Source Source, int Value);