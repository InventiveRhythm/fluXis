using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupSlider : SetupEntry
{
    public float Default { get; init; } = 8;
    public Action<float> OnChange { get; init; } = _ => { };

    private Bindable<float> bindable;
    private FluXisSpriteText valueText;

    public SetupSlider(string title)
        : base(title)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindable.BindValueChanged(onChange);
    }

    private void onChange(ValueChangedEvent<float> e)
    {
        valueText.Text = e.NewValue.ToStringInvariant("0.0");
        OnChange.Invoke((float)Math.Round(e.NewValue, 1));
    }

    protected override Drawable CreateRightTitle()
    {
        return valueText = new FluXisSpriteText
        {
            Text = $"{Default.ToStringInvariant("0.0")}",
            WebFontSize = 16,
            Alpha = .8f
        };
    }

    protected override Drawable CreateContent()
    {
        bindable = new BindableFloat(Default)
        {
            MinValue = 1,
            Precision = 0.1f,
            MaxValue = 10
        };

        return new FluXisSlider<float>
        {
            RelativeSizeAxes = Axes.X,
            Height = 15,
            Step = 0.1f,
            Bindable = bindable
        };
    }
}
