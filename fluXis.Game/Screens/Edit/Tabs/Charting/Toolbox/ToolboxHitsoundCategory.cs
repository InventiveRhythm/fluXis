using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Shared.Toolbox;
using fluXis.Game.Screens.Gameplay.Audio.Hitsounds;
using Humanizer;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxHitsoundCategory : ToolboxCategory
{
    public List<ToolboxButton> Items { get; }

    public ToolboxHitsoundCategory()
    {
        Title = "Hitsound";
        ExtraTitle = "(Q-R)";
        Icon = FontAwesome6.Solid.Drum;

        Items = Hitsounding.Defaults.Select(s => new ToolboxHitsoundButton(s.Humanize(LetterCasing.Title), s)).Cast<ToolboxButton>().ToList();
    }

    protected override List<ToolboxButton> GetItems() => Items;
}
