using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Toolbar;
using fluXis.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game
{
    public class FluXisGame : FluXisGameBase
    {
        private ScreenStack screenStack;

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Children = new Drawable[]
            {
                new BackgroundStack(),
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                new Toolbar()
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new MenuScreen());
        }

        protected override void Update()
        {
            Conductor.Update(Time);
            base.Update();
        }
    }
}
