using System;
using Godot;
using Pact.Server.Contexts;
using Pact.Shared.State;

namespace Pact.Models.Triggers;

[GlobalClass]
public abstract partial class QueryTriggerModel : TriggerModel
{
    public override TriggerType TriggerType { get; } = TriggerType.OnQuery;
    public required Func<Context, bool> Predicate { get; set; }
}