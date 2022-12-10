using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Graphics.Background
{
    public class BackgroundStack : CompositeDrawable
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.FromHex("#1a1a20")
            };
        }
    }
}
