using Arch.Core;

namespace Pact.Server.Contexts;

public record Context(
    Entity Caster,
    Target Target,
    Source Source,
    DamageInstance? Damage = null
    );
    