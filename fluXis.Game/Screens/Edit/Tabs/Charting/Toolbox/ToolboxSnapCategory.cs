using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxSnapCategory : ToolboxCategory
{
    public ToolboxSnapCategory()
    {
        Title = "Snap";
        Icon = FontAwesome6.Solid.Magnet;
    }

    protected override List<ToolboxButton> GetItems() => ChartingContainer.SNAP_DIVISORS.Select(v => new ToolboxSnapButton(v)).ToList<ToolboxButton>();
}
