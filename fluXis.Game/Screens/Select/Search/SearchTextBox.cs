using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchTextBox : FluXisTextBox
{
    private readonly SearchBar search;

    protected override Color4 SelectionColour => FluXisColors.Accent2;

    public SearchTextBox(SearchBar search)
    {
        this.search = search;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        TextContainer.Padding = new MarginPadding { Left = 10, Right = 40 };
        PlaceholderText = "Search...";
        CornerRadius = 10;
    }

    private void updateSearch()
    {
        search.Screen.Filters.Query = Text;
    }

    protected override void OnUserTextAdded(string added)
    {
        updateSearch();
        base.OnUserTextAdded(added);
    }

    protected override void OnUserTextRemoved(string removed)
    {
        updateSearch();
        base.OnUserTextRemoved(removed);
    }
}
