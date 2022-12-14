using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Toolbar
{
    public class Toolbar : Container
    {
        private bool visible = true;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Size = new Vector2(1, 80);

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#222228")
                },
                new ToolbarProfile()
            };

            Toggle();
        }

        public void Toggle()
        {
            visible = !visible;

            if (!visible)
                this.MoveToY(-Height, 500, Easing.OutQuint);
            else
                this.MoveToY(0, 500, Easing.OutQuint);
        }
    }
}
