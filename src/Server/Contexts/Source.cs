using Pact.Shared.State;

namespace Pact.Server.Contexts;

public record struct Source(int CasterId, AbilitySlot AbilityIndex, int EffectIndex);