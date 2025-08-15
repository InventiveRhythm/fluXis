using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticeControl : GridContainer
{
    private BindableNumber<int> bind { get; }
    private BindableNumber<int> other { get; }

    private FluXisSpriteText time { get; }
    private ForcedHeightText percentage { get; }

    private InputManager input;

    public FooterPracticeControl(string title, BindableNumber<int> bind, BindableNumber<int> other)
    {
        this.bind = bind;
        this.other = other;

        Width = 250;
        RelativeSizeAxes = Axes.Y;
        ColumnDimensions = new Dimension[]
        {
            new(GridSizeMode.Absolute, 32),
            new(),
            new(GridSizeMode.Absolute, 32)
        };

        Content = new[]
        {
            new Drawable[]
            {
                new Button(FontAwesome6.Solid.Minus, () => changeValue(-1)),
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        new ForcedHeightText
                        {
                            Text = title,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 12,
                            Height = 9,
                            Alpha = .6f
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.X,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = 18,
                            Child = time = new FluXisSpriteText
                            {
                                Text = "1:24",
                                WebFontSize = 24,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }
                        },
                        percentage = new ForcedHeightText
                        {
                            Text = "50%",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 16,
                            Height = 12,
                            Alpha = .8f
                        }
                    }
                },
                new Button(FontAwesome6.Solid.Plus, () => changeValue(1))
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        input = GetContainingInputManager();
    }

    private bool changeValue(int change)
    {
        var shift = input.CurrentState.Keyboard.ShiftPressed ? 10 : 1;
        var ctrl = input.CurrentState.Keyboard.ControlPressed ? 6 : 1;

        var val = bind.Value;
        bind.Value += change * shift * ctrl;

        var changed = val != bind.Value;
        time.MoveToY(-change * (changed ? 6 : 2)).MoveToY(0, 600, Easing.OutElasticQuarter);
        return changed;
    }

    protected override void Update()
    {
        base.Update();

        var max = Math.Max(bind.MaxValue, other.MaxValue);

        time.Text = TimeUtils.Format(bind.Value * 1000, false);
        percentage.Text = $"{bind.Value / (float)max * 100:0}%";
    }

    private partial class Button : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }
        private Sample holdSample;

        private Func<bool> action { get; }
        private HoverLayer hover { get; }
        private Box flash { get; }

        private bool isMouseDown = false;
        private double mouseDownTime = 0;
        private double lastClickTime = 0;
        private double lastSampleTime = 0;
        private const int hold_duration = 250;
        private const int invoke_interval = 25;
        private const int sample_interval = 50;

        public Button(IconUsage icon, Func<bool> action)
        {
            this.action = action;

            RelativeSizeAxes = Axes.Both;
            CornerRadius = 8;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Text,
                    Alpha = 0,
                },
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(16)
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        { 
            holdSample = samples.Get("UI/slider-tick");
        }

        protected override void Update()
        {
            if (isMouseDown)
            {
                double timeSinceMouseDown = Clock.CurrentTime - mouseDownTime;

                if (timeSinceMouseDown >= hold_duration)
                {
                    if (Clock.CurrentTime - lastClickTime >= invoke_interval)
                    {
                        action?.Invoke();

                        if (Clock.CurrentTime - lastSampleTime >= sample_interval)
                        {
                            holdSample.Play();
                            lastSampleTime = Clock.CurrentTime;
                        }

                        lastClickTime = Clock.CurrentTime;
                    }
                }
            }
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {   
            isMouseDown = true;
            mouseDownTime = Clock.CurrentTime;
            lastClickTime = Clock.CurrentTime;
            flash.FadeTo(0.6f, 500, Easing.OutQuint);
            action?.Invoke();
            samples.Click();
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            isMouseDown = false;
            flash.FadeOut(500, Easing.OutQuint);
            base.OnMouseUp(e);
        }
    }
}
