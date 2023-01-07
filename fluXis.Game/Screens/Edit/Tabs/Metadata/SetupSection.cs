using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata
{
    public class SetupSection : FillFlowContainer
    {
        public SetupSection(string title)
        {
            Direction = FillDirection.Vertical;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Add(new MetadataSectionHeader(title));
        }
    }

    public class MetadataSectionHeader : SpriteText
    {
        public MetadataSectionHeader(string text)
        {
            Text = text;
            Font = new FontUsage("Quicksand", 32, "SemiBold");
        }
    }

    public class SetupTextBox : Container
    {
        private readonly TextBox textBox;

        public string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public SetupTextBox(string name)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = name,
                    Font = new FontUsage("Quicksand", 24, "SemiBold"),
                    Margin = new MarginPadding { Top = 5 },
                    Origin = Anchor.TopRight,
                    X = 90
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 30,
                    Margin = new MarginPadding { Top = 5 },
                    Child = textBox = new TextBox(),
                    Padding = new MarginPadding { Left = 100 },
                }
            };
        }

        private class TextBox : BasicTextBox
        {
            public TextBox()
            {
                CornerRadius = 5;
                Masking = true;
                RelativeSizeAxes = Axes.Both;
                BackgroundUnfocused = Colour4.FromHex("#2a2a30");
                BackgroundFocused = Colour4.FromHex("#818182");
                BackgroundCommit = Colour4.FromHex("#acacad");
            }

            protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
            {
                AutoSizeAxes = Axes.Both,
                Child = new SpriteText
                {
                    Text = c.ToString(),
                    Font = new FontUsage("Quicksand", 24, "SemiBold").With(size: CalculatedTextSize)
                }
            };
        }
    }
}
