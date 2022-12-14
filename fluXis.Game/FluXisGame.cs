using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Toolbar;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game
{
    public class FluXisGame : FluXisGameBase
    {
        private ScreenStack screenStack;
        private Toolbar toolbar;

        [Cached]
        private MapStore mapStore = new MapStore();

        [Cached]
        private BackgroundStack backgroundStack = new BackgroundStack();

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            Discord.Init();

            mapStore.LoadMaps(storage);

            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Children = new Drawable[]
            {
                backgroundStack,
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                toolbar = new Toolbar()
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(new Conductor());
            screenStack.Push(new SelectScreen());
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    toolbar.Toggle();
                    return true;
            }

            return base.OnKeyDown(e);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
