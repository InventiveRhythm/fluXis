using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI.Buttons;

/// <summary>
/// A button with just an icon and no text.
/// </summary>
public partial class MenuSmallButton : MenuButtonBase
{
    protected override Drawable[] CreateContent()
    {
        return new Drawable[]
        {
            new SpriteIcon
            {
                Icon = Icon,
                Size = new Vector2(32),
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Shear = new Vector2(-SHEAR_AMOUNT, 0),
            }
        };
    }
}
