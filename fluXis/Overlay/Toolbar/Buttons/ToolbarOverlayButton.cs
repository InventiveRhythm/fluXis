using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Toolbar.Buttons;

public partial class ToolbarOverlayButton : ToolbarButton
{
    public OverlayContainer Overlay
    {
        init
        {
            Action = value.ToggleVisibility;
            overlayState.BindTo(value.State);
        }
    }

    private Bindable<Visibility> overlayState { get; } = new();

    protected override void LoadComplete()
    {
        base.LoadComplete();

        overlayState.BindValueChanged(stateChanged, true);
    }

    private void stateChanged(ValueChangedEvent<Visibility> e) => SetLineState(e.NewValue == Visibility.Visible);
}
