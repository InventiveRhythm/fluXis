using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchTextBox : BasicTextBox
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
        Placeholder.Font = new FontUsage("Quicksand", 40, "Bold");
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
            Font = new FontUsage("Quicksand", 40, "Bold"),
        }
    };

    private void updateSearch()
    {
        search.Screen.Search = Text;
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
