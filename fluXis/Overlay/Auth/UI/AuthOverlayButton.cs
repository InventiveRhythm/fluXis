using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Auth.UI;

public partial class AuthOverlayButton : FluXisButton
{
    public AuthOverlayButton(string text)
    {
        Text = text;
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Color = FluXisColors.Highlight;
        TextColor = FluXisColors.Background2;
        FontSize = FluXisSpriteText.GetWebFontSize(14);
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
    }
}
