using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox.Hitsound;

public partial class ToolboxHitsoundCategory : ToolboxCategory
{
    private readonly List<ToolboxButton> items;

    public ToolboxHitsoundCategory()
    {
        Title = "Hitsound";
        Icon = FontAwesome6.Solid.Drum;

        items = new List<ToolboxButton>
        {
            new ToolboxHitsoundButton("Normal", "normal"),
            new ToolboxHitsoundButton("Kick", "kick"),
            new ToolboxHitsoundButton("Clap", "clap"),
            new ToolboxHitsoundButton("Snare", "snare"),
        };
    }

    protected override List<ToolboxButton> GetItems() => items;
}
