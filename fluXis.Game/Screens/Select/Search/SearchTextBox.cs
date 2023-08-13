using fluXis.Game.Graphics;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchTextBox : FluXisTextBox
{
    public SearchFilters Search { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundInactive = FluXisColors.Background2;
        BackgroundActive = FluXisColors.Background2;
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        PlaceholderText = "Click to Search...";
        CornerRadius = 0;
    }

    private void updateSearch()
    {
        Search.Query = Text;
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
