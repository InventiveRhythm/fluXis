using fluXis.Configuration;
using fluXis.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public abstract partial class PointSettingsBase : CompositeDrawable
{
    private Bindable<bool> compactMode;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        compactMode = config.GetBindable<bool>(FluXisSetting.EditorCompactMode);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        compactMode.BindValueChanged(compactChanged, true);
        FinishTransforms(true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        compactMode.ValueChanged -= compactChanged;
    }

    private void compactChanged(ValueChangedEvent<bool> v)
    {
        var compact = v.NewValue;

        if (AutoSizeAxes != Axes.Y)
            this.ResizeHeightTo(compact ? 24 : 32, Styling.TRANSITION_MOVE, Easing.OutQuint);

        InternalChildren.ForEach(x => x.ScaleTo(compact ? 0.75f : 1f, Styling.TRANSITION_MOVE, Easing.OutQuint));
        CompactModeChanged(compact);
    }

    protected virtual void CompactModeChanged(bool compact) { }
}
