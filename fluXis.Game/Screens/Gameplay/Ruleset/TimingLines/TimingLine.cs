using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines
{
    public class TimingLine : Box
    {
        private readonly TimingLineManager manager;
        private readonly int position;

        public TimingLine(TimingLineManager manager, int position)
        {
            this.manager = manager;
            this.position = position;
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
            int delta = position - Conductor.CurrentTime;
            Y = -60 - 0.5f * (delta * manager.HitObjectManager.ScrollSpeed);

            base.Update();
        }
    }
}
