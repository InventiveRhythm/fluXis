using fluXis.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Text;

public partial class ForcedHeightText : CompositeDrawable
{
    public LocalisableString Text
    {
        get => text.Text;
        set => text.Text = value;
    }

    public ColourInfo TextColor
    {
        get => text.Colour;
        set => text.Colour = value;
    }

    public float FontSize
    {
        get => text.FontSize;
        set => text.FontSize = value;
    }

    public float WebFontSize
    {
        set
        {
            Height = value;
            FontSize = FluXisSpriteText.GetWebFontSize(value);
        }
    }

    public bool Shadow
    {
        get => text.Shadow;
        set => text.Shadow = value;
    }

    private FluXisSpriteText text { get; }

    public ForcedHeightText(bool truncate = false, float max = 0)
    {
        if (!truncate || max > 0)
            AutoSizeAxes = Axes.X;

        InternalChild = text = truncate ? new TruncatingText() : new FluXisSpriteText();
        text.Anchor = Anchor.CentreLeft;
        text.Origin = Anchor.CentreLeft;

        if (max > 0)
            text.MaxWidth = max;

        if (truncate && max <= 0)
            text.RelativeSizeAxes = Axes.X;
    }
}
