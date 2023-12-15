using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Skinning.Default.Health;

public partial class DefaultHealthBackground : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40, 500);
        CornerRadius = 10;
        Masking = true;
        BorderColour = Colour4.Black.Opacity(.8f);
        BorderThickness = 4;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            }
        };
    }
}
