using System;
using System.Numerics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupSlider<T> : SetupEntry
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public string Format { get; init; } = "0.0";

    public Action<T> OnChange { get; init; } = _ => { };

    private readonly BindableNumber<T> bindable;
    private FluXisSpriteText valueText;

    public SetupSlider(string title, T value, T min, T max, T precision)
        : this(title, new BindableNumber<T>(value) { MinValue = min, MaxValue = max, Precision = precision })
    {
    }

    public SetupSlider(string title, BindableNumber<T> bind)
        : base(title)
    {
        bindable = bind;
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

    protected override Drawable CreateRightTitle() => valueText = new FluXisSpriteText
    {
        Text = $"{Convert.ToDouble(bindable.Value).ToStringInvariant(Format)}",
        WebFontSize = 16,
        Alpha = .8f
    };

    protected override Drawable CreateContent() => new FluXisSlider<T>
    {
        RelativeSizeAxes = Axes.X,
        Height = 16,
        Step = bindable.Precision,
        Bindable = bindable,
        CustomColor = Theme.Highlight
    };
}
