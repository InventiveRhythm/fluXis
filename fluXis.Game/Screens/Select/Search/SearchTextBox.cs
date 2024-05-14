using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Localization;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchTextBox : FluXisTextBox
{
    [Resolved]
    private SearchFilters filters { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundInactive = FluXisColors.Background2;
        BackgroundActive = FluXisColors.Background2;
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        PlaceholderText = LocalizationStrings.SongSelect.SearchPlaceholder;
        CornerRadius = 0;
    }

    private void updateSearch()
    {
        filters.Query = Text;
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
