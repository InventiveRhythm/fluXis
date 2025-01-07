using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Tabs;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Overlay.MapSet.Tabs;

public partial class MapSetScoreTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.ArrowTrendUp;
    public override string Title => "Scores";
}
