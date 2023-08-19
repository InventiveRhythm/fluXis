using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface;

public partial class FluXisSlider<T> : Container where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Bindable { get; init; }
    public float Step { get; init; }
    public bool PlaySample { get; init; } = true;

    private BindableNumber<T> bindableNumber => Bindable as BindableNumber<T>;
    private BasicSliderBar<T> sliderBar;
    private ClickableSpriteIcon leftIcon;
    private ClickableSpriteIcon rightIcon;

    private Sample valueChange;
    private Bindable<double> valueChangePitch;
    private bool firstPlay = true;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        Height = 20;

        InternalChildren = new Drawable[]
        {
            leftIcon = new ClickableSpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronLeft,
                Size = new Vector2(10),
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
                Size = new Vector2(10),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 5 },
                Action = () => changeValue(1)
            }
        };

        valueChange = samples.Get("UI/slider-scroll");
        valueChange?.AddAdjustment(AdjustableProperty.Frequency, valueChangePitch = new BindableDouble(1f));
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

        if (valueChange != null && PlaySample && !firstPlay)
        {
            valueChangePitch.Value = 1f + percent * .4f;
            valueChange.Play();
        }
        else firstPlay = false;
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
