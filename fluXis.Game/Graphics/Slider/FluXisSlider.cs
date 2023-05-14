using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Graphics.Slider;

public partial class FluXisSlider<T> : Container where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Bindable { get; init; }
    public float Step { get; init; }

    private BindableNumber<T> bindableNumber => Bindable as BindableNumber<T>;
    private BasicSliderBar<T> sliderBar;
    private ClickableSpriteIcon leftIcon;
    private ClickableSpriteIcon rightIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 20;

        InternalChildren = new Drawable[]
        {
            leftIcon = new ClickableSpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronLeft,
                Size = new(10),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 5 },
                Action = () => changeValue(-1)
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 20, Right = 20 },
                Child = new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = sliderBar = new BasicSliderBar<T>
                    {
                        RelativeSizeAxes = Axes.Both,
                        Current = Bindable,
                        BackgroundColour = FluXisColors.Accent2.Opacity(.2f),
                        SelectionColour = FluXisColors.Accent2,
                        KeyboardStep = Step
                    }
                }
            },
            rightIcon = new ClickableSpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronRight,
                Size = new(10),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 5 },
                Action = () => changeValue(1)
            }
        };
    }

    protected override void LoadComplete()
    {
        bindableNumber?.BindValueChanged(updateValue, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        bindableNumber.ValueChanged -= updateValue;
        base.Dispose(isDisposing);
    }

    private void updateValue(ValueChangedEvent<T> e)
    {
        float percent = (float)(bindableNumber.Value.ToDouble(null) - bindableNumber.MinValue.ToDouble(null)) / (float)(bindableNumber.MaxValue.ToDouble(null) - bindableNumber.MinValue.ToDouble(null));
        Colour4 colour = FluXisColors.AccentGradient.Interpolate(new Vector2(percent, 1));
        sliderBar.SelectionColour = colour;
        sliderBar.BackgroundColour = colour.Opacity(.2f);

        leftIcon.Enabled.Value = bindableNumber.Value.CompareTo(bindableNumber.MinValue) > 0;
        rightIcon.Enabled.Value = bindableNumber.Value.CompareTo(bindableNumber.MaxValue) < 0;
    }

    private void changeValue(int by)
    {
        if (bindableNumber != null)
        {
            if (by > 0)
                bindableNumber.Add(Step);
            else
                bindableNumber.Add(-Step);
        }
    }
}
