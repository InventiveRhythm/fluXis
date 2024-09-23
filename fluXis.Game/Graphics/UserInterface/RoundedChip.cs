using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Graphics.UserInterface;

public partial class RoundedChip : CircularContainer
{
    public string Text { get; init; } = "text";
    public float FontSize { get; set; } = FluXisSpriteText.GetWebFontSize(12);
    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public ColourInfo TextColour { get; init; } = FluXisColors.Text.Opacity(.75f);
    public ColourInfo BackgroundColour { get; init; } = FluXisColors.Background2;
    public float SidePadding { get; init; } = 12;

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
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = BackgroundColour
            },
            new FluXisSpriteText
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
