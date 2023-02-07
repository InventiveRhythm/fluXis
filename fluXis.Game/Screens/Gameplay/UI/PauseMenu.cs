using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.UI
{
    public partial class PauseMenu : CompositeDrawable
    {
        public GameplayScreen Screen;

        public PauseMenu(GameplayScreen screen)
        {
            Screen = screen;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[] { };
        }
    }
}
