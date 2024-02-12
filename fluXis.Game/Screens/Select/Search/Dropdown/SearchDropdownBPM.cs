using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Select.Search.Dropdown;

public partial class SearchDropdownBPM : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "BPM",
                FontSize = 24,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                X = 5
            },
            new TextBox()
        };
    }

    private partial class TextBox : FluXisTextBox
    {
        [Resolved]
        private SearchFilters filters { get; init; }

        private int bpm;

        public TextBox()
        {
            Width = 200;
            Height = 30;
            CornerRadius = 10;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            PlaceholderText = "0";

            OnTextChanged += update;
        }

        private void update()
        {
            string text = Text;
            SearchFilters.Type type = SearchFilters.Type.Exact;

            if (text.StartsWith(">"))
            {
                type = SearchFilters.Type.Over;
                text = text[1..];
            }
            else if (text.StartsWith("<"))
            {
                type = SearchFilters.Type.Under;
                text = text[1..];
            }

            if (text == "") text = "0";

            bpm = int.TryParse(text, out int result) ? result : 0;

            filters.BPM = bpm;
            filters.BPMType = type;
        }
    }
}
