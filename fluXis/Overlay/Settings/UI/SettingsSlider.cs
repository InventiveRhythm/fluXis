using System;
using System.Numerics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Input;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using Vector2 = osuTK.Vector2;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem, IKeyBindingHandler<FluXisGlobalKeybind>
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; }

    public T? Step { get; init; }
    private T step => Step ?? (Bindable as BindableNumber<T>)?.Precision ?? T.Zero;

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
                Step = step,
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

    protected override bool ActivateFocus()
    {
        if (!Enabled)
            return true;

        HoldingFocus = true;
        return true;
    }

    protected override void DeactivateFocus() => HoldingFocus = false;

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (!HoldingFocus)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.PreviousGroup:
                Bindable.Value -= step;
                return true;

            case FluXisGlobalKeybind.NextGroup:
                Bindable.Value += step;
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}
