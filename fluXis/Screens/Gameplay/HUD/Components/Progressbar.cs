using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class Progressbar : GameplayHUDComponent
{
    private Bar bar;
    private FluXisSpriteText currentTimeText;
    private FluXisSpriteText percentText;
    private FluXisSpriteText timeLeftText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(20);

        InternalChildren = new Drawable[]
        {
            bar = new Bar { Progressbar = this },
            currentTimeText = new FluXisSpriteText
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Y = 18,
                X = 10,
                FontSize = 32
            },
            percentText = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 18,
                FontSize = 32
            },
            timeLeftText = new FluXisSpriteText
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Y = 18,
                X = -10,
                FontSize = 32
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        int timeLeft = (int)((Deps.MapInfo.EndTime - Deps.CurrentTime) / Deps.PlaybackRate);
        int totalTime = (int)((Deps.MapInfo.EndTime - Deps.MapInfo.StartTime) / Deps.PlaybackRate);

        int currentTime = (int)((Deps.Ruleset.ParentClock.CurrentTime - Deps.MapInfo.StartTime) / Deps.PlaybackRate);
        int catchupTime = (int)((Deps.CurrentTime - Deps.MapInfo.StartTime) / Deps.PlaybackRate);

        float percent = (float)currentTime / totalTime;
        float catchupPercent = (float)catchupTime / totalTime;

        bar.Progress = Math.Clamp(percent, 0, 1);
        bar.CatchUpProgress = Math.Clamp(catchupPercent, 0, 1);

        percentText.Text = $"{(int)Math.Clamp(percent * 100, 0, 100)}%";
        currentTimeText.Text = TimeUtils.Format(currentTime, false);
        timeLeftText.Text = TimeUtils.Format(timeLeft, false);
    }

    private partial class Bar : CircularContainer
    {
        public float Progress
        {
            set
            {
                if (value < 0 || !float.IsFinite(value)) value = 0;
                if (value > 1) value = 1;

                bar.ResizeWidthTo(value, 200);
            }
        }

        public float CatchUpProgress
        {
            set
            {
                if (value < 0 || !float.IsFinite(value)) value = 0;
                if (value > 1) value = 1;

                catchup.ResizeWidthTo(value, 200);
            }
        }

        public Progressbar Progressbar { get; init; }

        private Circle bar;
        private Circle catchup;
        private bool catchingUp;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 10;
            Masking = true;
            Colour = Theme.Text;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = .2f,
                    Blending = BlendingParameters.Additive
                },
                bar = new Circle
                {
                    RelativeSizeAxes = Axes.Both
                },
                catchup = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Highlight,
                    Alpha = 0
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            var delta = Math.Abs(bar.Width - catchup.Width);
            var shouldShow = delta > 0.004f;

            if (shouldShow == catchingUp)
                return;

            catchingUp = shouldShow;
            catchup.FadeTo(catchingUp ? .8f : 0, 200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            var progress = e.MousePosition.X / DrawWidth;
            if (progress < 0) progress = 0;
            if (progress > 1) progress = 1;

            var startTime = Progressbar.Deps.MapInfo.StartTime;
            var endTime = Progressbar.Deps.MapInfo.EndTime;
            double newTime = startTime + (endTime - startTime) * progress;
            Progressbar.Deps.Ruleset.ParentClock.Seek(newTime);
            return true;
        }
    }
}
