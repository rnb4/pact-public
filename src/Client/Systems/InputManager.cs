using Godot;
using Pact.AutoLoad;
using Pact.Client.Views;
using Pact.Shared.Messages.Requests;
using Pact.Shared.Snapshot;
using AbilityRequest = Pact.Shared.Messages.Requests.AbilityRequest;
using AbilitySlot = Pact.Shared.State.AbilitySlot;

namespace Pact.Client.Systems;

[GlobalClass]
public partial class InputManager : Node2D
{
    [Export] public Node2D? Cursor { get; set; }
    
    public EntityId HoveredEntityId { get; set; }
    private View? PreviousHoveredEntity { get; set; }

    private static readonly float HeroPickRadius = 1f.PixelsToUnits();

    public override void _Process(double delta)
    {
        HoveredEntityId = UpdateHoveredEntity();
        HighlightHoveredEntity();
        if (Cursor != null) Cursor.Position = GetGlobalMousePosition();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Multiplayer.HasMultiplayerPeer()) return;
        if (Multiplayer.IsServer()) return;

        var localEntityId = NetworkManager.Instance.LocalEntityId;
        if (localEntityId == EntityId.None) return;

        if (Input.IsActionJustPressed("basic_attack") && HoveredEntityId != EntityId.None && HoveredEntityId != localEntityId)
        {
            NetworkManager.Instance.SendRequest(new AbilityRequest
            {
                EntityId = (EntityId)localEntityId,
                Slot = AbilitySlot.Basic,
                TargetEntityId = HoveredEntityId,
            });
            return;
        }

        if (Input.IsActionJustPressed("move"))
        {
            NetworkManager.Instance.SendRequest(new MoveRequest
            {
                EntityId = localEntityId,
                Destination = GetGlobalMousePosition(),
            });
            return;
        }

        AbilitySlot? slot = null;
        if (Input.IsActionJustPressed("ability_primary")) slot = AbilitySlot.Primary;
        else if (Input.IsActionJustPressed("ability_secondary")) slot = AbilitySlot.Secondary;
        else if (Input.IsActionJustPressed("ability_tertiary")) slot = AbilitySlot.Tertiary;
        else if (Input.IsActionJustPressed("ability_ultimate")) slot = AbilitySlot.Ultimate;
        
        if (!slot.HasValue) return;

        NetworkManager.Instance.SendRequest(new AbilityRequest
        {
            EntityId = (EntityId)localEntityId,
            Slot = slot.Value,
        });
    }

    private EntityId UpdateHoveredEntity()
    {
        var best = EntityId.None;
        var bestDistance = HeroPickRadius;
        foreach (var hero in ClientRoot.Instance.Heroes)
        {
            var distance = hero.Position.DistanceTo(GetGlobalMousePosition());
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = hero.EntityId;
            }
        }
        return best;
    }

    private void HighlightHoveredEntity()
    {
        ClientRoot.Instance.TryGetView(HoveredEntityId, out var view);
        if (view == PreviousHoveredEntity) return;
        PreviousHoveredEntity?.UnHighlight();
        view?.Highlight();
        PreviousHoveredEntity = view;
    }
}