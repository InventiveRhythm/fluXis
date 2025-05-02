using System;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Toolbar.Buttons;

public partial class ToolbarOverlayButton : ToolbarButton
{
    public OverlayContainer Overlay
    {
        init
        {
            Action = () =>
            {
                value.ToggleVisibility();
                OnVisibilityToggle?.Invoke(value.State.Value);
            };
            overlayState.BindTo(value.State);
        }
    }

    [CanBeNull]
    public Action<Visibility> OnVisibilityToggle { get; init; }

    private Bindable<Visibility> overlayState { get; } = new();

    protected override void LoadComplete()
    {
        base.LoadComplete();

        overlayState.BindValueChanged(stateChanged, true);
    }

    private void stateChanged(ValueChangedEvent<Visibility> e) => SetLineState(e.NewValue == Visibility.Visible);
}
