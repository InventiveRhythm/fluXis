using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface;

public partial class RoundedChip : CircularContainer
{
    private LocalisableString text = "text";
    private ColourInfo backgroundColour = FluXisColors.Background2;
    private ColourInfo textColour = FluXisColors.Text.Opacity(.75f);

    public LocalisableString Text
    {
        get => text;
        set
        {
            text = value;

            if (spriteText != null)
                spriteText.Text = value;
        }
    }

    public ColourInfo BackgroundColour
    {
        get => backgroundColour;
        set
        {
            backgroundColour = value;

            if (background != null)
                background.Colour = value;
        }
    }

    public ColourInfo TextColour
    {
        get => textColour;
        set
        {
            textColour = value;

            if (spriteText != null)
                spriteText.Colour = value;
        }
    }

    public float FontSize { get; set; } = FluXisSpriteText.GetWebFontSize(12);
    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float SidePadding { get; init; } = 12;

    private Box background;
    private FluXisSpriteText spriteText;

    public RoundedChip()
    {
        AutoSizeAxes = Axes.X;
        Height = 20;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = BackgroundColour
            },
            spriteText = new FluXisSpriteText
            {
                Text = Text,
                FontSize = FontSize,
                Colour = TextColour,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Horizontal = SidePadding }
            }
        };
    }
}
