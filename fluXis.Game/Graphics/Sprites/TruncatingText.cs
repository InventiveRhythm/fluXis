using fluXis.Game.Overlay.Mouse;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.Sprites;

public partial class TruncatingText : FluXisSpriteText, IHasTextTooltip
{
    public LocalisableString Tooltip => Text;

    public override bool HandlePositionalInput => IsTruncated && tooltip;

    private bool tooltip { get; }

    public TruncatingText(bool showTooltip = true)
    {
        tooltip = showTooltip;
        ((SpriteText)this).Truncate = true;
    }
}
