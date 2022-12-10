using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class Progressbar : GameplayHUDElement
    {
        public Progressbar(GameplayScreen screen)
            : base(screen)
        {
        }

        private Box bar;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            RelativeSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                bar = new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 10,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                }
            };
        }

        protected override void Update()
        {
            bar.Height = (float)Conductor.Time / Screen.Map.EndTime;

            base.Update();
        }
    }
}
