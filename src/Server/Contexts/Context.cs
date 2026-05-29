using Target = Pact.Server.Contexts.Target;

namespace Pact.Server.Contexts;

public abstract record Context(Source Source, Target Target);