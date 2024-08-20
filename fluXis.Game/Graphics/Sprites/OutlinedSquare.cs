using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Graphics.Sprites;

public partial class OutlinedSquare : Container
{
    public OutlinedSquare()
    {
        BorderColour = Colour4.White;
        BorderThickness = 20;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }
}
