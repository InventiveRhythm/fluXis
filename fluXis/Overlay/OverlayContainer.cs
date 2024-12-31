using fluXis.Audio;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay;

public abstract partial class OverlayContainer : VisibilityContainer
{
    protected override bool StartHidden => true;
    protected virtual bool PlaySamples => true;

    [Resolved]
    private UISamples samples { get; set; }

    protected override void UpdateState(ValueChangedEvent<Visibility> state)
    {
        base.UpdateState(state);

        if (!PlaySamples)
            return;

        samples.Overlay(state.NewValue != Visibility.Visible);
    }
}
