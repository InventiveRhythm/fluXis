using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooterPlayButton : FooterCornerButton
{
    protected override string ButtonText => "Play";
    protected override IconUsage Icon => FontAwesome.Solid.Play;
    protected override Colour4 ButtonColor => FluXisColors.Accent2;

    public SelectFooterPlayButton()
    {
        Anchor = Anchor.BottomRight;
        Origin = Anchor.BottomRight;
    }
}
