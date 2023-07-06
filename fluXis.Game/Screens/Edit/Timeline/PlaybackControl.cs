using System;
using System.Linq;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
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

    private static readonly float[] playback_speeds = { .25f, .5f, .75f, 1f };

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
            new GridContainer
            {
                ColumnDimensions = new Dimension[]
                {
                    new(),
                    new(),
                    new(),
                    new()
                },
                RelativeSizeAxes = Axes.Y,
                Width = 140,
                Margin = new MarginPadding { Left = 50 },
                Content = new[]
                {
                    playback_speeds.Select(speed => new PlaybackSpeedText { Speed = speed }).ToArray()
                }
            }
        };
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;

        if (e.ControlPressed)
        {
            switch (e.Key)
            {
                case Key.Minus or Key.KeypadMinus:
                    int index = Array.IndexOf(playback_speeds, getClosetPlaybackSpeed(clock.Rate));
                    clock.Rate = index == 0 ? playback_speeds[0] : playback_speeds[index - 1];
                    return true;

                case Key.Plus or Key.KeypadPlus:
                    index = Array.IndexOf(playback_speeds, getClosetPlaybackSpeed(clock.Rate));
                    clock.Rate = index == playback_speeds.Length - 1 ? playback_speeds[^1] : playback_speeds[index + 1];
                    return true;
            }
        }

        return false;
    }

    private static float getClosetPlaybackSpeed(double speed) => playback_speeds.Aggregate((x, y) => Math.Abs(x - speed) < Math.Abs(y - speed) ? x : y);

    private partial class PlayButton : Container
    {
        [Resolved]
        private EditorClock clock { get; set; }

        private SpriteIcon icon;
        private Box background;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.Centre;
            CornerRadius = 5;
            Masking = true;

            Children = new Drawable[]
            {
                background = new Box
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
            background.FadeTo(.2f, 200);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeOut(200);
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

            background.FadeTo(.4f).FadeTo(.2f, 200);

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
        [Resolved]
        private EditorClock clock { get; set; }

        public float Speed
        {
            get => speed;
            init
            {
                speed = value;
                Text = $"{speed * 100}%";
            }
        }

        private readonly float speed;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Origin = Anchor.Centre;
            FontSize = 16;
        }

        protected override void Update()
        {
            Colour = clock.Rate == Speed ? FluXisColors.Text : FluXisColors.Text2;
        }

        protected override bool OnClick(ClickEvent e)
        {
            clock.Rate = Speed;
            return base.OnClick(e);
        }
    }
}
