using EntityId = Pact.Shared.State.EntityId;

namespace Pact.Server.Contexts;

// identity only
public record Source(EntityId Caster, int AbilityIndex, int EffectIndex);