using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchBar : FillFlowContainer
{
    public SelectScreen Screen;

    public SearchBar(SelectScreen screen)
    {
        Screen = screen;

        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 5);
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Padding = new MarginPadding { Left = 20, Top = 10, Right = 10 };

        Children = new Drawable[]
        {
            new Container
            {
                CornerRadius = 5,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Height = 40,
                Child = new SearchTextBox(this)
            },
            new Container
            {
                CornerRadius = 5,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Height = 40,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            },
        };
    }

    private partial class SearchTextBox : BasicTextBox
    {
        private readonly SearchBar search;

        protected override Color4 SelectionColour => FluXisColors.Accent2;

        public SearchTextBox(SearchBar search)
        {
            this.search = search;

            RelativeSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 5;
            TextContainer.Padding = new MarginPadding { Left = 10 };
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
}
