using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Sides;

public partial class ResultsSideList : FillFlowContainer
{
    public ResultsSideList()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        Spacing = new Vector2(16);
    }
}
