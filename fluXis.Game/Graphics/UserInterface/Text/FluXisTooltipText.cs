using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Text;

/// <summary>
/// FluXisSpriteText with a tooltip.
/// </summary>
public partial class FluXisTooltipText : FluXisSpriteText, IHasTextTooltip
{
    public LocalisableString Tooltip { get; init; }

    protected override bool OnHover(HoverEvent e) => true;
}
