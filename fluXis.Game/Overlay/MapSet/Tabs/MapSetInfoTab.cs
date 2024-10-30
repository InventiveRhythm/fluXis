using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Tabs;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.MapSet.Tabs;

public partial class MapSetInfoTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.Info;
    public override string Title => "Info";
}
