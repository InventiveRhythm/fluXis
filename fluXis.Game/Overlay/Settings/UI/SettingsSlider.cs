using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Slider;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; } = false;
    public float Step { get; init; } = .01f;

    private SpriteText valueLabel;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = Label,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            valueLabel = new SpriteText
            {
                Text = ValueLabel,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 410 }
            },
            new FluXisSlider<T>
            {
                Bindable = Bindable,
                Step = Step,
                Width = 400,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            }
        });
    }

    protected override void LoadComplete()
    {
        Bindable.BindValueChanged(onValueChanged, true);
    }

    private void onValueChanged(ValueChangedEvent<T> value)
    {
        valueLabel.Text = DisplayAsPercentage
            ? $"{Math.Round(Bindable.Value.ToDouble(null) * 100)}%"
            : ValueLabel.Replace("{value}", Math.Round(Bindable.Value.ToDouble(null), 2).ToString(CultureInfo.InvariantCulture));
    }
}
