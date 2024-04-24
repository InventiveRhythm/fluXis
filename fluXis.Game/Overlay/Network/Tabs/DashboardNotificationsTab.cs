using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardNotificationsTab : DashboardWipTab
{
    public override string Title => "Notifications";
    public override IconUsage Icon => FontAwesome6.Solid.Bell;
    public override DashboardTabType Type => DashboardTabType.Notifications;
}
