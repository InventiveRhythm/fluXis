using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooterBackButton : FooterCornerButton
{
    protected override string ButtonText => "Back";
    protected override IconUsage Icon => FontAwesome.Solid.ChevronLeft;

    public SelectFooterBackButton()
    {
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
    }
}
