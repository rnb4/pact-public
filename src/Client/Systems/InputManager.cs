using Godot;
using Pact.AutoLoad;
using Pact.Client.Views;
using Pact.Shared.Messages.Requests;
using Pact.Shared.State.Entities;
using AbilitySlot = Pact.Shared.State.AbilitySlot;

namespace Pact.Client.Systems;

[GlobalClass]
public partial class InputManager : Node2D
{
    private static readonly float HeroPickRadius = 1f.UnitsToPixels();

    private HeroView? _hoveredHero;

    public override void _Process(double delta)
    {
        ClientRoot.Instance.TryGetView(UpdateHoveredEntity(), out var view);
        var next = view as HeroView;
        if (next == _hoveredHero) return;
        _hoveredHero?.UnHighlight();
        _hoveredHero = next;
        _hoveredHero?.Highlight();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Multiplayer.HasMultiplayerPeer()) return;
        if (Multiplayer.IsServer()) return;

        var localEntityId = NetworkManager.Instance.LocalEntityId;
        if (localEntityId == -1) return;

        if (Input.IsActionJustPressed("basic_attack") && _hoveredHero != null && _hoveredHero.Id != localEntityId)
        {
            NetworkManager.Instance.SendRequest(new CastRequest
            {
                EntityId = localEntityId,
                Slot = AbilitySlot.Basic,
                TargetEntityId = _hoveredHero.Id
            });
            return;
        }

        if (Input.IsActionJustPressed("move"))
        {
            NetworkManager.Instance.SendRequest(new MoveRequest
            {
                EntityId = localEntityId,
                Destination = GetGlobalMousePosition()
            });
            return;
        }

        AbilitySlot? slot = null;
        if (Input.IsActionJustPressed("ability_primary")) slot = AbilitySlot.Primary;
        else if (Input.IsActionJustPressed("ability_secondary")) slot = AbilitySlot.Secondary;
        else if (Input.IsActionJustPressed("ability_tertiary")) slot = AbilitySlot.Tertiary;
        else if (Input.IsActionJustPressed("ability_ultimate")) slot = AbilitySlot.Ultimate;

        if (!slot.HasValue) return;

        NetworkManager.Instance.SendRequest(new CastRequest
        {
            EntityId = localEntityId,
            Slot = slot.Value
        });
    }

    private int UpdateHoveredEntity()
    {
        var best = -1;
        var bestDistance = HeroPickRadius;
        foreach (var hero in ClientRoot.Instance.GameState.Query<HeroState>())
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
}