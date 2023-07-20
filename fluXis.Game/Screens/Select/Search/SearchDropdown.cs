using fluXis.Game.Screens.Select.Search.Dropdown;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchDropdown : FillFlowContainer
{
    public SearchDropdown(SearchFilters filters)
    {
        Padding = new MarginPadding(10) { Top = 50 };
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);

        InternalChildren = new Drawable[]
        {
            new SearchDropdownBPM { Filters = filters },
            new SearchDropdownStatus { Filters = filters }
        };
    }
}
