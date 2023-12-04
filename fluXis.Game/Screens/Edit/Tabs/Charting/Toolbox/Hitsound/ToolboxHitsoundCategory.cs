using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox.Hitsound;

public partial class ToolboxHitsoundCategory : ToolboxCategory
{
    private readonly List<ToolboxButton> items;

    public ToolboxHitsoundCategory()
    {
        Title = "Hitsound";
        Icon = FontAwesome.Solid.Drum;

        items = new List<ToolboxButton>
        {
            new ToolboxHitsoundButton("Normal", "normal"),
            new ToolboxHitsoundButton("Drum", "drum"),
            new ToolboxHitsoundButton("Clap", "clap")
        };
    }

    protected override List<ToolboxButton> GetItems() => items;
}
