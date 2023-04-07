using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchTextBox : FluXisTextBox
{
    private readonly SearchBar search;

    protected override Color4 SelectionColour => FluXisColors.Accent2;

    public SearchTextBox(SearchBar search)
    {
        this.search = search;

        RelativeSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 5;
        TextContainer.Padding = new MarginPadding { Left = 10, Right = 40 };
        BackgroundUnfocused = FluXisColors.Background2;
        BackgroundFocused = FluXisColors.Hover;
        PlaceholderText = "Search...";
        Placeholder.Colour = Colour4.Gray;
        Placeholder.Font = FluXisFont.Default(40);
    }

    protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
    {
        Height = 40,
        AutoSizeAxes = Axes.X,
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
        Child = new SpriteText
        {
            Text = c.ToString(),
            Font = FluXisFont.Default(40)
        }
    };

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
