using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.UserInterface.Text;

/// <summary>
/// FluXisSpriteText with a tooltip.
/// </summary>
public partial class FluXisTooltipText : FluXisSpriteText, IHasTextTooltip
{
    public string Tooltip { get; init; }

    protected override bool OnHover(HoverEvent e) => true;
}
