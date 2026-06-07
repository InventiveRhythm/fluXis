using fluXis.Configuration;
using fluXis.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.UI.Variable;

public abstract partial class EditorVariableBase : CompositeDrawable, IHasTooltip
{
    public string Text { get; init; }
    public LocalisableString TooltipText { get; init; } = string.Empty;

    public Bindable<bool> Enabled { get; init; } = new(true);
    public bool HideWhenDisabled { get; init; } = false;

    private Bindable<bool> compactMode;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        compactMode = config.GetBindable<bool>(FluXisSetting.EditorCompactMode);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(e =>
        {
            this.FadeTo(e.NewValue ? 1f : HideWhenDisabled ? 0 : .4f, 200);
            UpdateEnabledState(e.NewValue);
        }, true);

        compactMode.BindValueChanged(compactChanged, true);
        FinishTransforms(true);
    }

    protected virtual void UpdateEnabledState(bool state) { }

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
