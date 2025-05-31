using System;
using System.Numerics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Vector2 = osuTK.Vector2;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; }
    public float Step { get; init; } = .01f;

    protected override bool IsDefault => Bindable.IsDefault;

    private FluXisSpriteText valueLabel;

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        AutoSizeAxes = Axes.Both,
        Direction = FillDirection.Horizontal,
        Spacing = new Vector2(8),
        Children = new Drawable[]
        {
            valueLabel = new FluXisSpriteText
            {
                Text = ValueLabel,
                WebFontSize = 14,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new FluXisSlider<T>
            {
                Bindable = Bindable,
                Step = Step,
                Width = 400,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            }
        }
    };

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
