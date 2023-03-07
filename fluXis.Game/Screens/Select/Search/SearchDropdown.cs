using fluXis.Game.Screens.Select.Search.Dropdown;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchDropdown : FillFlowContainer
{
    public SearchDropdown(SearchFilters filters)
    {
        Padding = new MarginPadding(10);
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;

        Add(new SearchDropdownBPM(filters));
    }
}
