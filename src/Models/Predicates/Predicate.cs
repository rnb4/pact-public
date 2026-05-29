using System;
using Godot;
using Pact.Server.Contexts;

namespace Pact.Models.Predicates;

public abstract partial class Predicate : Resource
{
    public abstract Func<Context, bool> Evaluate { get; init; }
}