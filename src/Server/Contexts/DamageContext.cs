using Pact.Shared.State;
using Source = Pact.Server.Contexts.Source;
using Target = Pact.Server.Contexts.Target;

namespace Pact.Server.Contexts;

public record DamageContext(Source Source, Target Target, DamageInstance DamageInstance) : Context(Source, Target);