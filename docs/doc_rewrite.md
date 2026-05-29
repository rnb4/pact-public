# MOBA

## Abilities

**Ability**: Class; A collection of `Triggers`.

**Trigger**: Class; A firing condition.
```cs
public class Trigger
{
    public IEffect Effect { get; init; }
    public Predicate? Predicate { get; init; }
    public ResourceCost? Cost { get; init; }

    public void Fire(Context context)
    {
        if (Predicate != null && !Predicate.Evaluate(context.Source, context.Target)) return;
        if (Cost != null)
        {
            var resourceManager = context.Source.Caster.GetComponent<ResourceManager>();
            if (resourceManager == null || !resourceManager.CanAfford(Cost)) return;
            resourceManager.Spend(Cost);
        }
        Effect.Execute(context.Source, context.Target);
    }
}
```

**TriggerType**: Enum;
- OnAdd
- OnRemove
- OnActivate
- OnQuery
- OnInterval
- OnEvent

**Predicate**: Class; Checks a condition.
```cs
public abstract class Predicate
{
    public abstract bool Evaluate(Context context);
}
```

**Effect**: Interface; A state modulation.
```cs
public interface IEffect
{
    void Execute(Context context);
}
```

**Context**: record; A `Source`, `Target`, and other optional fields.
```cs
public record Context(Source Source, Target Target);
public record DamageContext(Source Source, Target Target, DamageInstance Damage) : Context(Source, Target);
```

**Source**: record; An `Entity` and `Ability`.
```cs
public record Source(Entity Caster, Ability Ability, Effect Effect);
```

**Target**: record; A `Vector3` and optional `Entity`.
```cs
public record Target(Vector3 Position, Entity? Entity);
```

**ResourceCost**
```cs
public class ResourceCost
{
    public float Amount { get; init; }
    public ResourceType ResourceType { get; init; }
}
```

### Statuses

**Status**: Class; An `Ability` with `Source` stacks and an optional duration. Displayed on status bar.
```cs
public class Status
{
    public Source Source { get; init; }
    public Ability Ability { get; init; }
    public ApplicationBehavior ApplicationBehavior { get; init; }
    public TimeoutBehavior TimeoutBehavior { get; init; }
    public Texture2D Icon { get; init; }
    public int MaxStacks { get; init; }
    public int CurrentStacks { get; set; } = 0;
}
```

**TimeoutBehavior**: abstract class; What happends when stacks are removed (usually by duration expiring).
```cs
public abstract record TimeoutBehavior;
public record RemoveAll : TimeoutBehavior;
public record RemoveN(int Count) : TimeoutBehavior;
```

**ApplicationBehavior**: enum; What happens when new stacks are applied.
- KeepDuration
- RefreshDurationIfLonger

## Entities

**Entity**: A thing that is. Heroes, Minions, Monsters, Towers, Inhibitors, Nexus, Projectiles, Summons, Etc.
```cs
public class Entity
{
    public 
}
```

### Components

**ResourceManager**: Handles `Resources` 

**Resource** class; Health, Mana, Etc.
```cs
public class Resource
{
    public float Current { get; set; }
    public float Max { get; set; }
    public float RegenAmount { get; set; }
    public float RegenInterval { get; set; }
}
```

**Team**: Enum; What team the `Entity` is on.
- Neutral
- Red
- Blue

**AbilityHandler**: A set of `Abilities`.
```cs
public class AbilityHandler
{
    void Add(Ability ability);
    void Add(Status status);
    void Remove(Ability ability);
    void Remove(int slot);
    void Remove(Status status);
    void Remove(Source source); // remove status by source

    void Activate(int slot, Context context);
    void QueryAll(Context context);
    void QueryAbilities(Context context);
    void QueryStatuses(Context context);
}
```

**Position**: The `Entity` exists in space.

**Vision**: The `Entity` can see.

**Movement**: The `Entity` can move.

**Controller**: The `Entity` is controlled.

**Renderer**: Sprite3D; The `Entity` displays on the screen.

### Stats

**Stat** class; base value and `StatModifierSet`. Final value = `(base + ΣflatAdd) × (1 + ΣpercentAdd) × ΠpercentMultiply`

**StatModifierSet**: class; a collection of `StatModifiers`

**StatModifier**: record; A `Source`, `StatModifierOperation`, and value.

**StatModifierOperation**: enum; 
- Flat
- PercentAdd
- PercentMultiply

## Networking

### Snapshots
**GameStateSnapshot**: Record: 'GameState' sent to clients, filtered by vision.

**GameState**: Record: `Entity` registry and global state of the server.

### Notifications

**Notification**: One shot events with no state trace. Reliably sent to clients.

**AnnouncementNotifcation**: Announces something. Kill streaks, events, inhibitor respawns, etc.

**KillNotification**: Updates kill feed.

### Requests

**Request**: Sent from client to server to do things.

**MoveRequest**

**AbilityRequest**

**StopRequest**

**CursorUpdateRequest**

**BuyItemRequest**

**SellItemRequest**

**UpgradeAbilityRequest**

## Models

**TriggerModel**: Resource; Defines a `Trigger` with a `TriggerType`, `Predicate?`, and `Effect`.
```cs
public abstract partial class TriggerModel : Resource
{
    [Export] public PredicateModel? Predicate { get; init; }
    [Export] public EffectModel? Effect { get; init; }
    public abstract TriggerType TriggerType { get; init; }
}

public partial class OnQueryTriggerModel : TriggerModel
{
    public override TriggerType TriggerType { get; init; } = TriggerType.OnQuery;
}

public partial class IntervalTriggerModel : TriggerModel
{
    [Export] public int TickInterval { get; init; }
    public override TriggerType TriggerType { get; init; } = TriggerType.OnInterval;
}

public partial class EventTriggerModel : TriggerModel
{
    [Export] public EventId EventId { get; init; } // some enum
    public override TriggerType TriggerType { get; init; } = TriggerType.OnEvent;
}
```

**PredicateModel**: Resource; Defines a `Predicate`.
```cs
public abstract partial class PredicateModel : Resource
{
    public abstract Func<Source, Target, bool> Evaluate { get; init; }
}

public partial class OnHitPredicateModel : PredicateModel
{
    public override Func<Source, Target, bool> Evaluate { get; init; } =
        (source, target) => ...;
}
```

**EffectModel**: Resource; Defines an `Effect`.

**StatusModel**: Resource; Defines a `Status`.
```cs
public partial class StatusModel : Resource
{
    [Export] public AbilityModel Ability { get; init; }
    [Export] public ApplicationBehavior ApplicationBehavior { get; init; }
    [Export] public StatusTimeoutBehaviorModel TimeoutBehavior { get; init; }
    [Export] public Texture2D Icon { get; init; }
    [Export] public int MaxStacks { get; init; }
}
```

**StatusTimeoutBehaviorModel**: Resource; Define a `TimeoutBehavior`.
```cs
public partial class StatusTimeoutBehaviorModel : Resource;
public partial class RemoveAll : StatusTimeoutBehaviorModel;
public partial class RemoveN : StatusTimeoutBehaviorModel
{
    [Export] public int Count { get; } = 1;
}
```

**ResourceCostModel**
```cs
public partial class ResourceCostModel : Resource
{
    [Export] public float Amount { get; init; }
    [Export] public ResourceType ResourceType { get; init; }
}
```