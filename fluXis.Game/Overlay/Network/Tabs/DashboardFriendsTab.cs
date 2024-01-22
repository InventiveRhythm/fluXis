using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardFriendsTab : DashboardWipTab
{
    public override string Title => "Friends";
    public override IconUsage Icon => FontAwesome6.Solid.UserGroup;
}
