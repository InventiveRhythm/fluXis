using fluXis.Graphics.Sprites;
using osu.Framework.Graphics;
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

    public float FontSize
    {
        get => text.FontSize;
        set => text.FontSize = value;
    }

    public float MaxWidth
    {
        get => text.MaxWidth;
        set => text.MaxWidth = value;
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

    public ForcedHeightText(bool truncate = false)
    {
        if (!truncate)
            AutoSizeAxes = Axes.X;

        InternalChild = text = truncate ? new TruncatingText { RelativeSizeAxes = Axes.X } : new FluXisSpriteText();
        text.Anchor = Anchor.CentreLeft;
        text.Origin = Anchor.CentreLeft;
    }
}
