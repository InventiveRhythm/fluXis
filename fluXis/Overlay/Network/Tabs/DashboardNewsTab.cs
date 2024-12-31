using fluXis.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardNewsTab : DashboardWipTab
{
    public override string Title => "News";
    public override IconUsage Icon => FontAwesome6.Solid.Newspaper;
    public override DashboardTabType Type => DashboardTabType.News;
}
