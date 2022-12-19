using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Online.Overlay
{
    public class OverlayChat : Container
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Width = 0.8f;
            Height = 400;
            Padding = new MarginPadding { Left = 10 };
            CornerRadius = 10;
            Masking = true;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#222228")
                },
            };
        }
    }
}
