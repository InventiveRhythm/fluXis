using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class PlaybackControl : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    private static readonly float[] playback_speeds = { .4f, .6f, .8f, 1f };

    private BindableFloat rateBindable;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(30),
                Margin = new MarginPadding { Left = 10 },
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Child = new PlayButton()
            },
            new FillFlowContainer
            {
                Width = 140,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 50 },
                Children = new Drawable[]
                {
                    new GridContainer
                    {
                        ColumnDimensions = new Dimension[] { new(), new(), new(), new() },
                        RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) },
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Content = new[]
                        {
                            playback_speeds.Select(speed => new PlaybackSpeedText
                            {
                                Speed = speed,
                                OnClickAction = ChangeRate,
                                ClosestCheckAction = GetClosestPlaybackSpeed
                            }).ToArray()
                        }
                    },
                    new FluXisSlider<float>
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 10,
                        Bindable = rateBindable = new BindableFloat
                        {
                            MinValue = playback_speeds[0],
                            MaxValue = playback_speeds[^1],
                            Default = 1f,
                            Value = 1f,
                            Precision = 0.05f
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        rateBindable.BindValueChanged(e => clock.Rate = e.NewValue, true);
    }

    public void ChangeRate(float rate) => rateBindable.Value = rate;

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;

        if (e.ControlPressed)
        {
            switch (e.Key)
            {
                case Key.Minus or Key.KeypadMinus:
                    int index = Array.IndexOf(playback_speeds, GetClosestPlaybackSpeed());
                    clock.Rate = index == 0 ? playback_speeds[0] : playback_speeds[index - 1];
                    return true;

                case Key.Plus or Key.KeypadPlus:
                    index = Array.IndexOf(playback_speeds, GetClosestPlaybackSpeed());
                    clock.Rate = index == playback_speeds.Length - 1 ? playback_speeds[^1] : playback_speeds[index + 1];
                    return true;
            }
        }

        return false;
    }

    public float GetClosestPlaybackSpeed() => playback_speeds.Aggregate((x, y) => Math.Abs(x - clock.Rate) < Math.Abs(y - clock.Rate) ? x : y);

    private partial class PlayButton : Container
    {
        [Resolved]
        private EditorClock clock { get; set; }

        [Resolved]
        private UISamples samples { get; set; }

        private SpriteIcon icon;
        private Box hover;
        private Box flash;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.Centre;
            CornerRadius = 5;
            Masking = true;

            Children = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.Play,
                    Size = new Vector2(16)
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(.2f, 50);
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(200);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left) return false;

            this.ScaleTo(0.9f, 2000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (e.Button != MouseButton.Left) return;

            this.ScaleTo(1f, 800, Easing.OutElastic);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (e.Button != MouseButton.Left) return false;

            flash.FadeOutFromOne(1000, Easing.OutQuint);
            samples.Click();

            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();

            return true;
        }

        protected override void Update()
        {
            icon.Icon = clock.IsRunning ? FontAwesome.Solid.Pause : FontAwesome.Solid.Play;
        }
    }

    private partial class PlaybackSpeedText : FluXisSpriteText
    {
        public Func<float> ClosestCheckAction { get; init; }
        public Action<float> OnClickAction { get; init; }

        public float Speed
        {
            get => speed;
            init
            {
                speed = value;
                Text = $"{(int)(speed * 100)}%";
            }
        }

        [Resolved]
        private UISamples samples { get; set; }

        private readonly float speed;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Origin = Anchor.Centre;
            FontSize = 16;
        }

        protected override void Update()
        {
            Colour = ClosestCheckAction() == Speed ? FluXisColors.Text : FluXisColors.Text2;
        }

        protected override bool OnClick(ClickEvent e)
        {
            OnClickAction(Speed);
            samples.Click();
            return base.OnClick(e);
        }
    }
}
