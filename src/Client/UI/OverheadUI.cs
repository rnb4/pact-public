using Godot;
using Pact.Client.Views;

namespace Pact.Client.UI;

[GlobalClass]
public partial class OverheadUI : Control
{
    [Export] private Vector2 _offset = Vector2.Zero;
    private View _view = null!;
    
    public override void _Ready()
    {
        _view = GetParent<View>();
        _view.TreeExiting += QueueFree;
        CallDeferred(Node.MethodName.Reparent, ClientRoot.Instance.OverheadUI);
    }

    public override void _Process(double delta)
    {
        GlobalPosition = _view.GetGlobalTransformWithCanvas().Origin - (Size * 0.5f) + _offset * GetViewport().GetCamera2D().Zoom;
    }
}