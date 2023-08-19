using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Lighting;

public partial class DefaultColumnLighing : Box
{
    public DefaultColumnLighing()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        RelativeSizeAxes = Axes.X;
        Height = 232;
        Alpha = 0;
    }

    public void UpdateColor(int lane, int maxLanes)
    {
        var color = FluXisColors.GetLaneColor(lane, maxLanes);
        Colour = ColourInfo.GradientVertical(color.Opacity(0), color);
    }
}
