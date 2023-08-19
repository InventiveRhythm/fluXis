using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultHitLine : Box
{
    public DefaultHitLine()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.TopCentre;
        Height = 3;
        Colour = ColourInfo.GradientHorizontal(FluXisColors.Accent3, FluXisColors.Accent);
    }
}
