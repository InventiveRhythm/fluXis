using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardNewsTab : DashboardWipTab
{
    public override string Title => "News";
    public override IconUsage Icon => FontAwesome6.Solid.Newspaper;
}
