using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Screens.Edit.Tabs.Design.Points;

public partial class DesignSidebar : PointsSidebar
{
    protected override PointsList CreatePointsList() => new DesignPointsList();
}
