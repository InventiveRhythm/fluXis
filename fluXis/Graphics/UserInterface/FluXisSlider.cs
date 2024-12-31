using System.Numerics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using Vector2 = osuTK.Vector2;

namespace fluXis.Graphics.UserInterface;

public partial class FluXisSlider<T> : Container where T : struct, INumber<T>, IMinMaxValue<T>
{
    public Bindable<T> Bindable { get; init; }
    public float Step { get; init; }
    public bool PlaySample { get; init; } = true;
    public bool ShowArrowButtons { get; init; } = true;

    private float pad => ShowArrowButtons ? 20 : 0;

    private BindableNumber<T> bindableNumber => Bindable as BindableNumber<T>;
    private ClickableSpriteIcon leftIcon;
    private ClickableSpriteIcon rightIcon;

    private double lastSampleTime;
    private Sample valueChange;
    private Bindable<double> valueChangePitch;
    private bool firstPlay = true;

    public FluXisSlider()
    {
        Height = 20;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        InternalChildren = new Drawable[]
        {
            leftIcon = new ClickableSpriteIcon
            {
                Icon = FontAwesome6.Solid.AngleLeft,
                Size = new Vector2(10),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 5 },
                Action = () => changeValue(-1),
                Alpha = ShowArrowButtons ? 1 : 0
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = pad, Right = pad },
                Child = new FluXisSliderBar(Height)
                {
                    RelativeSizeAxes = Axes.Both,
                    Current = Bindable,
                    KeyboardStep = Step
                }
            },
            rightIcon = new ClickableSpriteIcon
            {
                Icon = FontAwesome6.Solid.AngleRight,
                Size = new Vector2(10),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 5 },
                Action = () => changeValue(1),
                Alpha = ShowArrowButtons ? 1 : 0
            }
        };

        valueChange = samples.Get("UI/slider-tick");
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
        var percent = bindableNumber.NormalizedValue;

        leftIcon.Enabled.Value = bindableNumber.Value.CompareTo(bindableNumber.MinValue) > 0;
        rightIcon.Enabled.Value = bindableNumber.Value.CompareTo(bindableNumber.MaxValue) < 0;

        if (valueChange != null && PlaySample && !firstPlay)
        {
            if (Time.Current - lastSampleTime < 50) return;

            valueChangePitch.Value = .7f + percent * .6f;
            valueChange.Play();

            lastSampleTime = Time.Current;
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

    private partial class FluXisSliderBar : SliderBar<T>
    {
        private Box background { get; }
        private Box bar { get; }

        public FluXisSliderBar(float height)
        {
            CornerRadius = height / 2;
            Masking = true;

            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Primary,
                    Alpha = .2f
                },
                bar = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Primary
                }
            };
        }

        protected override void UpdateValue(float value)
        {
            Colour4 colour = FluXisColors.AccentGradient.Interpolate(new Vector2(value, 1));
            bar.FadeColour(colour, 400, Easing.OutQuint).ResizeWidthTo(value, 400, Easing.OutQuint);
            background.FadeColour(colour, 400, Easing.OutQuint);
        }
    }
}
