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

    public Colour4? CustomColor { get; set; }

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
                Action = () => changeValue(-1)
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 20 },
                Child = new FluXisSliderBar(this, Height)
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
                Action = () => changeValue(1)
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

        if (valueChange != null && !firstPlay && IsPresent)
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
        private FluXisSlider<T> parent { get; }
        private Box background { get; }
        private Box bar { get; }

        public FluXisSliderBar(FluXisSlider<T> parent, float height)
        {
            this.parent = parent;

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

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.Add(() => FinishTransforms(true));
        }

        protected override void UpdateValue(float value)
        {
            var colour = parent.CustomColor ?? FluXisColors.AccentGradient.Interpolate(new Vector2(value, 1));
            bar.FadeColour(colour, 400, Easing.OutQuint).ResizeWidthTo(value, 400, Easing.OutQuint);
            background.FadeColour(colour, 400, Easing.OutQuint);
        }
    }
}
