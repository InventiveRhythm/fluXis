using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxSnapCategory : ToolboxCategory
{
    public ToolboxSnapCategory()
    {
        Title = "Snap";
        Icon = FontAwesome.Solid.Magnet;
    }

    protected override List<ToolboxButton> GetItems() => ChartingContainer.SNAP_DIVISORS.Select(v => new ToolboxSnapButton(v)).ToList<ToolboxButton>();
}
