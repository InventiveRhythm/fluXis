using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Graphics.Sprites.Text;

/// <summary>
/// FluXisSpriteText with a tooltip.
/// </summary>
public partial class FluXisTooltipText : FluXisSpriteText, IHasTooltip
{
    public LocalisableString TooltipText { get; init; }

    protected override bool OnHover(HoverEvent e) => !string.IsNullOrEmpty(TooltipText.ToString());
}
