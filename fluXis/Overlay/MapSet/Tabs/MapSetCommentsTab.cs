using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Tabs;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Overlay.MapSet.Tabs;

public partial class MapSetCommentsTab : TabContainer
{
    public override IconUsage Icon => Phosphor.Bold.ChatsCircle;
    public override string Title => "Comments";
}
