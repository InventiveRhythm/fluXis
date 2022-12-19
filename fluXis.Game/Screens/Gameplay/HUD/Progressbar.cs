using System;
using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class Progressbar : GameplayHUDElement
    {
        public Progressbar(GameplayScreen screen)
            : base(screen)
        {
        }

        private Box bar;
        private SpriteText currentTimeText;
        private SpriteText percentText;
        private SpriteText timeLeftText;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.TopLeft;
            Origin = Anchor.TopLeft;
            RelativeSizeAxes = Axes.X;

            InternalChildren = new Drawable[]
            {
                bar = new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 10,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft
                },
                currentTimeText = new SpriteText
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Y = 20,
                    X = 20,
                    Font = new FontUsage("Quicksand", 32f, "SemiBold")
                },
                percentText = new SpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Y = 20,
                    Font = new FontUsage("Quicksand", 32f, "SemiBold")
                },
                timeLeftText = new SpriteText
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Y = 20,
                    X = -20,
                    Font = new FontUsage("Quicksand", 32f, "SemiBold")
                },
            };
        }

        protected override void Update()
        {
            float speed = Conductor.Speed == 0 ? 1 : Conductor.Speed;
            int currentTime = (int)((Conductor.CurrentTime - Screen.Map.StartTime) / speed);
            int timeLeft = (int)((Screen.Map.EndTime - Conductor.CurrentTime) / speed);
            int totalTime = (int)((Screen.Map.EndTime - Screen.Map.StartTime) / speed);
            bool negative = currentTime < 0;

            bar.Width = (float)currentTime / totalTime;
            percentText.Text = $"{(int)((float)currentTime / totalTime * 100)}%";
            currentTimeText.Text = (negative ? "-" : "") + $"{currentTime / 1000 / 60}:{Math.Abs(currentTime / 1000 % 60):00}";
            timeLeftText.Text = $"{timeLeft / 1000 / 60}:{timeLeft / 1000 % 60:00}";

            base.Update();
        }
    }
}
