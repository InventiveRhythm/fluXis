using System;
using System.Numerics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsSlider<T> : Container, IHasTooltip
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public override bool PropagateNonPositionalInputSubTree => Enabled.Value;
    public override bool PropagatePositionalInputSubTree => Enabled.Value;

    public string Text { get; init; }
    public LocalisableString TooltipText { get; init; } = string.Empty;
    public string Formatting { get; init; } = "0.##";
    public T CurrentValue { get; init; }
    public Action<T> OnValueChanged { get; init; }

    public T Step { get; init; } = (T)Convert.ChangeType(0.1f, typeof(T));
    public T Min { get; init; }
    public T Max { get; init; }

    public Bindable<T> Bindable { get; set; }

    public Bindable<bool> Enabled { get; init; } = new(true);

    private FluXisSpriteText valText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        Bindable ??= new BindableNumber<T>
        {
            Default = CurrentValue,
            Value = CurrentValue,
            MinValue = Min,
            MaxValue = Max,
            Precision = Step
        };

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            },
            valText = new FluXisSpriteText
            {
                X = -150,
                Text = $"{CurrentValue}".Replace(',', '.'),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                WebFontSize = 16
            },
            new FluXisSlider<T>
            {
                Width = 150,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Bindable = Bindable,
                Step = (float)Convert.ToDouble(Step)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Bindable.BindValueChanged(valueChanged);

        Enabled.BindValueChanged(e => this.FadeTo(e.NewValue ? 1f : .4f, 200), true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= valueChanged;
    }

    private void valueChanged(ValueChangedEvent<T> e)
    {
        // round to step
        var val = Convert.ToDouble(e.NewValue);
        val = Math.Round(val / Convert.ToDouble(Step)) * Convert.ToDouble(Step);

        valText.Text = val.ToStringInvariant(Formatting);
        OnValueChanged?.Invoke(e.NewValue);
    }
}
