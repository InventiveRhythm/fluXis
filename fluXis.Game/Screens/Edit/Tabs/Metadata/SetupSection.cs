using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class SetupSection : FillFlowContainer
{
    public SetupSection(string title)
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Add(new MetadataSectionHeader(title));
    }
}

public partial class MetadataSectionHeader : SpriteText
{
    public MetadataSectionHeader(string text)
    {
        Text = text;
        Font = FluXisFont.Default(32);
    }
}

public partial class SetupTextBox : Container
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
                Font = FluXisFont.Default(24),
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
                Padding = new MarginPadding { Left = 100 }
            }
        };
    }

    private partial class TextBox : FluXisTextBox
    {
        protected override Color4 SelectionColour => FluXisColors.Accent2;

        public TextBox()
        {
            CornerRadius = 5;
            Masking = true;
            RelativeSizeAxes = Axes.Both;
            BackgroundUnfocused = FluXisColors.Surface;
            BackgroundFocused = FluXisColors.Hover;
            BackgroundCommit = FluXisColors.Click;
        }

        protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
        {
            AutoSizeAxes = Axes.Both,
            Child = new SpriteText
            {
                Text = c.ToString(),
                Font = FluXisFont.Default(CalculatedTextSize)
            }
        };
    }
}
