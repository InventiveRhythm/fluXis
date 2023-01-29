using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs
{
    public class ComposeTab : EditorTab
    {
        public ComposeTab(Editor screen)
            : base(screen)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = "This is still work in progress! Check back later.",
                            Font = new FontUsage("QuickSand", 48, "Bold")
                        }
                    }
                }
            };
        }
    }
}
