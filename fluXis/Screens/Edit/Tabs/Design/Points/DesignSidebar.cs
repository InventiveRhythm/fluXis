using fluXis.Graphics.UserInterface.Text;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points;

public partial class DesignSidebar : PointsSidebar
{
    protected override PointsList CreatePointsList() => new DesignPointsList();

    protected override Drawable CreateClosedContent() => new FluXisTextFlow()
    {
        Text = "Hover or press\n tab to open.",
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        TextAnchor = Anchor.TopCentre
    };
}
