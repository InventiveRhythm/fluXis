using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines
{
    public partial class TimingLine : Box
    {
        private readonly TimingLineManager manager;
        public float ScrollVelocityTime { get; }

        public TimingLine(TimingLineManager manager, float time)
        {
            this.manager = manager;
            ScrollVelocityTime = time;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Colour = Colour4.White;
            RelativeSizeAxes = Axes.X;
            Height = 1;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
        }

        protected override void Update()
        {
            float delta = ScrollVelocityTime - manager.HitObjectManager.CurrentTime;
            Y = -60 - 0.5f * (delta * manager.HitObjectManager.ScrollSpeed);

            base.Update();
        }
    }
}
