namespace Pact.Server.Contexts;

public record DamageContext(Source Source, Target Target, DamageInstance DamageInstance) : Context(Source, Target);