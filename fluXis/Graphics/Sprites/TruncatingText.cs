using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Graphics.Sprites;

public partial class TruncatingText : FluXisSpriteText, IHasTooltip
{
    public LocalisableString TooltipText => Text;

    public override bool HandlePositionalInput => IsTruncated && tooltip;

    private bool tooltip { get; }

    public TruncatingText(bool showTooltip = true)
    {
        tooltip = showTooltip;
        ((SpriteText)this).Truncate = true;
    }
}
