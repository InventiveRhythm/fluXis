using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Menu.UI.Buttons;

/// <summary>
/// A button with just an icon and no text.
/// </summary>
public partial class MenuExitButton : MenuButtonBase
{
    private FluXisSpriteIcon icon;

    protected override Drawable[] CreateContent()
    {
        return new Drawable[]
        {
            icon = new FluXisSpriteIcon
            {
                Icon = Icon,
                Size = new Vector2(32),
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Shear = new Vector2(-SHEAR_AMOUNT, 0),
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        Background.FadeColour(FluXisColors.Red, 50);
        icon.FadeColour(FluXisColors.Background2, 50);
        Samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        Background.FadeColour(FluXisColors.Background2, 200);
        icon.FadeColour(FluXisColors.Text, 200);
    }
}
