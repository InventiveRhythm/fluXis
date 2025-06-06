using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Localization;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Select.Search;

public partial class SearchTextBox : FluXisTextBox
{
    [Resolved]
    private SearchFilters filters { get; set; }

    public override bool HandleLeftRightArrows => false;

    public SearchTextBox()
    {
        FontSize = FluXisSpriteText.GetWebFontSize(22);
    }

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
