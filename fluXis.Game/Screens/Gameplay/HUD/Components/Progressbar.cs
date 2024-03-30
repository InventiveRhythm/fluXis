using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Gameplay.Audio;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class Progressbar : GameplayHUDComponent
{
    [Resolved]
    private GameplayClock clock { get; set; }

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
                Y = 10,
                X = 10,
                FontSize = 32
            },
            percentText = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 10,
                FontSize = 32
            },
            timeLeftText = new FluXisSpriteText
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Y = 10,
                X = -10,
                FontSize = 32
            }
        };
    }

    protected override void Update()
    {
        int currentTime = (int)((clock.CurrentTime - Screen.Map.StartTime) / Screen.Rate);
        int timeLeft = (int)((Screen.Map.EndTime - clock.CurrentTime) / Screen.Rate);
        int totalTime = (int)((Screen.Map.EndTime - Screen.Map.StartTime) / Screen.Rate);
        float percent = (float)currentTime / totalTime;
        if (percent < 0) percent = 0;

        if (Screen.Map.StartTime == Screen.Map.EndTime)
            percent = 1;

        bar.Progress = percent;
        percentText.Text = $"{(int)Math.Clamp(percent * 100, 0, 100)}%";
        currentTimeText.Text = TimeUtils.Format(currentTime, false);
        timeLeftText.Text = TimeUtils.Format(timeLeft, false);

        base.Update();
    }

    private partial class Bar : CircularContainer
    {
        public float Progress
        {
            set
            {
                if (value < 0) value = 0;
                if (value > 1) value = 1;

                bar.ResizeWidthTo(value, 200);
            }
        }

        [Resolved]
        private GameplayClock clock { get; set; }

        public Progressbar Progressbar { get; init; }

        private Circle bar;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 10;
            Masking = true;

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
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            var progress = e.MousePosition.X / DrawWidth;
            if (progress < 0) progress = 0;
            if (progress > 1) progress = 1;

            var startTime = Progressbar.Screen.Map.StartTime;
            var endTime = Progressbar.Screen.Map.EndTime;
            double previousTime = clock.CurrentTime;
            double newTime = startTime + (endTime - startTime) * progress;

            Progressbar.Screen.OnSeek?.Invoke(previousTime, newTime);

            return true;
        }
    }
}
