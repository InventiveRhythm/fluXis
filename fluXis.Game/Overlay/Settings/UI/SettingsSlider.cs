using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Slider;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; }
    public float Step { get; init; } = .01f;

    private FluXisSpriteText valueLabel;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            valueLabel = new FluXisSpriteText
            {
                Text = ValueLabel,
                FontSize = 24,
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
            : ValueLabel.Replace("{value}", Math.Round(Bindable.Value.ToDouble(null), 2).ToStringInvariant());
    }
}
