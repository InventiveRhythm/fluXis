using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Tabs;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Overlay.MapSet.Tabs;

public partial class MapSetCommentsTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.Message;
    public override string Title => "Comments";
}
