using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.Sprites.Outline;

public partial class OutlinedCircle : CircularContainer
{
    public OutlinedCircle()
    {
        BorderColour = FluXisColors.Text;
        BorderThickness = 20;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }
}
