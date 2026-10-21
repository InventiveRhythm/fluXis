using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface;

public partial class RoundedChip : CircularContainer
{
    public LocalisableString Text { set => spriteText.Text = value; }
    public ColourInfo BackgroundColour { set => background.Colour = value; }
    public ColourInfo TextColour { set => spriteText.Colour = value; }

    public float WebFontSize { set => spriteText.WebFontSize = value; }
    public float SidePadding { set => spriteText.Margin = new MarginPadding { Horizontal = value }; }

    private readonly Box background;
    private readonly FluXisSpriteText spriteText;

    public RoundedChip()
    {
        AutoSizeAxes = Axes.X;
        Height = 20;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            spriteText = new FluXisSpriteText
            {
                Text = "LABEL",
                WebFontSize = 12,
                Colour = Theme.Text.Opacity(.75f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Horizontal = 12 }
            }
        };
    }
}
