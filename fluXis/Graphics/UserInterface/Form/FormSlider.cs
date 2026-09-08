using System.Numerics;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using Vector2 = osuTK.Vector2;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormSlider<T> : BaseFormComponent<T, FormSlider<T>>, IHasTooltip
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    LocalisableString IHasTooltip.TooltipText => Description ?? "";

    public string Formatting { get; init; } = "0.##";

    private FluXisSpriteText valueLabel;

    public FormSlider(LocalisableString label, T value, T min, T max)
        : this(label, new BindableNumber<T> { Value = value, MinValue = min, MaxValue = max })
    {
    }

    public FormSlider(LocalisableString label, BindableNumber<T> bind)
        : base(label, bind)
    {
        Bindable.ValueChanged += v => valueLabel!.Text = v.NewValue.ToString(Formatting, null);
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.Both,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(8),
        Padding = new MarginPadding(PADDING),
        Children =
        [
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 16,
                Children =
                [
                    LabelSprite = CreateDefaultLabel().WithAnchor(Anchor.CentreLeft),
                    valueLabel = CreateDefaultLabel().WithAnchor(Anchor.CentreRight).With(x => x.Text = Value.ToString(Formatting, null))
                ]
            },
            new InnerSliderBar { Current = Bindable }
        ]
    };

    private partial class InnerSliderBar : SliderBar<T>, IHasCursorType
    {
        CursorType IHasCursorType.Cursor => CursorType.SizeHorizontal;

        private readonly Container bar;
        private readonly Container fill;
        private readonly Container nub;

        private DebouncedSample valueChange;
        private Bindable<double> valueChangePitch;
        private bool firstPlay = true;

        public InnerSliderBar()
        {
            RelativeSizeAxes = Axes.X;
            Height = 16;

            bar = roundedRect(Theme.Background2)
                  .WithRelativeSize(Axes.Both)
                  .WithAnchor(Anchor.Centre);

            fill = roundedRect(Theme.Highlight).WithRelativeSize(Axes.Both);
            bar.Add(fill);

            InternalChildren =
            [
                bar,
                nub = roundedRect(Theme.Highlight).With(x =>
                {
                    x.Width = 8;
                    x.RelativePositionAxes = Axes.X;
                    x.WithAnchor(Anchor.CentreLeft, Anchor.Centre);
                })
            ];

            Container roundedRect(Colour4 color) => new()
            {
                CornerRadius = 4,
                Masking = true,
                Child = new Box { Colour = color }.WithRelativeSize(Axes.Both)
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            AddInternal(valueChange = new DebouncedSample(samples.Get("UI/slider-tick")));
            valueChange?.AddAdjustment(AdjustableProperty.Frequency, valueChangePitch = new BindableDouble(1f));
        }

        protected override void UpdateValue(float value)
        {
            fill.ResizeWidthTo(value, 400, Easing.OutQuint);
            nub.MoveToX(value, 400, Easing.OutQuint);

            if (valueChange != null && !firstPlay && (IsDragged || HasFocus))
            {
                valueChangePitch.Value = .7f + value * .6f;
                valueChange.Play();
            }
            else firstPlay = false;
        }

        protected override void OnFocus(FocusEvent e)
        {
            base.OnFocus(e);
            activate();
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            base.OnFocusLost(e);
            deactivate();
        }

        private void activate()
        {
            bar.ResizeHeightTo(0.5f, 200, Easing.OutBack);
            nub.ResizeHeightTo(16, 200, Easing.OutBack);
        }

        private void deactivate()
        {
            if (IsHovered || HasFocus)
                return;

            bar.ResizeHeightTo(1f, 200, Easing.OutBack);
            nub.ResizeHeightTo(0, 200, Easing.OutBack);
        }
    }
}
