using System.Collections.Generic;
using Godot;
using Pact.Shared.State.Entities;

namespace Pact.Client.UI;

[GlobalClass]
public partial class StatusIcon : TextureRect
{
    private static readonly Dictionary<string, Texture2D> IconCache = [];

    [Export] private ProgressBar _duration = null!;
    [Export] private Label _stacks = null!;

    private string _iconUid = "";

    public void Bind(StatusState status)
    {
        if (status.IconUid != _iconUid)
        {
            _iconUid = status.IconUid;
            Texture = LoadIcon(_iconUid);
        }

        _duration.Value = status.Duration > 0f ? status.RemainingDuration / status.Duration * 100f : 0f;
        _stacks.Text = status.CurrentStacks > 1 ? status.CurrentStacks.ToString() : "";
    }

    private static Texture2D? LoadIcon(string uid)
    {
        if (string.IsNullOrEmpty(uid)) return null;
        if (IconCache.TryGetValue(uid, out var cached)) return cached;
        var texture = GD.Load<Texture2D>(uid);
        IconCache[uid] = texture;
        return texture;
    }
}
