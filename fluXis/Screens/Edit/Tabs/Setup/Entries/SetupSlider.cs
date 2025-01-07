using System;
using System.Numerics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupSlider<T> : SetupEntry
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public T Default { get; init; }
    public string Format { get; init; } = "0.0";

    public T MinValue { get; init; }
    public T MaxValue { get; init; }
    public T Precision { get; init; }

    public Action<T> OnChange { get; init; } = _ => { };

    private Bindable<T> bindable;
    private FluXisSpriteText valueText;

    public SetupSlider(string title, T minValue, T maxValue, T precision)
        : base(title)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        Precision = precision;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindable.BindValueChanged(onChange);
    }

    private void onChange(ValueChangedEvent<T> e)
    {
        var num = Convert.ToDouble(e.NewValue);

        valueText.Text = num.ToStringInvariant(Format);
        OnChange.Invoke(e.NewValue);
    }

    protected override Drawable CreateRightTitle()
    {
        return valueText = new FluXisSpriteText
        {
            Text = $"{Convert.ToDouble(Default).ToStringInvariant(Format)}",
            WebFontSize = 16,
            Alpha = .8f
        };
    }

    protected override Drawable CreateContent()
    {
        bindable = new BindableNumber<T>(Default)
        {
            MinValue = MinValue,
            Precision = Precision,
            MaxValue = MaxValue
        };

        return new FluXisSlider<T>
        {
            RelativeSizeAxes = Axes.X,
            Height = 15,
            Step = 0.1f,
            Bindable = bindable
        };
    }
}
