using fluXis.Game.Graphics;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Select.Search.Dropdown;

public partial class SearchDropdownBPM : Container
{
    public SearchDropdownBPM(SearchFilters filters)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        AddRange(new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "BPM",
                FontSize = 24,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
            },
            new TextBox(filters)
        });
    }

    private partial class TextBox : FluXisTextBox
    {
        private readonly SearchFilters filters;
        private int bpm;

        public TextBox(SearchFilters filters)
        {
            this.filters = filters;

            Width = 200;
            Height = 30;
            Masking = true;
            CornerRadius = 5;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            BackgroundUnfocused = FluXisColors.Surface;
            BackgroundFocused = FluXisColors.Surface2;
            PlaceholderText = "0";
            Placeholder.Colour = Colour4.Gray;
            Placeholder.Font = FluXisSpriteText.GetFont(size: 24);
        }

        protected override void OnUserTextAdded(string added) => update();
        protected override void OnUserTextRemoved(string removed) => update();

        private void update()
        {
            string text = Text;
            SearchFilters.Type type = SearchFilters.Type.Exact;

            if (text.StartsWith(">"))
            {
                type = SearchFilters.Type.Over;
                text = text.Substring(1);
            }
            else if (text.StartsWith("<"))
            {
                type = SearchFilters.Type.Under;
                text = text.Substring(1);
            }

            if (text == "") text = "0";

            bpm = int.TryParse(text, out int result) ? result : 0;

            filters.BPM = bpm;
            filters.BPMType = type;
        }

        protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
        {
            Height = 24,
            AutoSizeAxes = Axes.X,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Child = new FluXisSpriteText
            {
                Text = c.ToString(),
                FontSize = 24
            }
        };
    }
}
