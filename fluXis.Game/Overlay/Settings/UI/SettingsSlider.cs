using System;
using System.Numerics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; }
    public float Step { get; init; } = .01f;

    protected override bool IsDefault => Bindable.IsDefault;

    private BindableNumber<T> bindableNumber => Bindable as BindableNumber<T>;

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

    protected override void Reset() => Bindable.SetDefault();

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Bindable.BindValueChanged(onValueChanged, true);
    }

    private void onValueChanged(ValueChangedEvent<T> value)
    {
        var percent = float.CreateTruncating(value.NewValue);

        valueLabel.Text = DisplayAsPercentage
            ? $"{Math.Round(percent * 100)}%"
            : ValueLabel.Replace("{value}", Math.Round(percent, 2).ToStringInvariant());
    }
}
