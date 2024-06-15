using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI.Buttons;

/// <summary>
/// A button with an icon and text.
/// </summary>
public partial class MenuLongButton : MenuButtonBase
{
    protected override Drawable[] CreateContent()
    {
        return new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8),
                Padding = new MarginPadding(20),
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = Icon,
                        Size = new Vector2(32),
                        Origin = Anchor.CentreLeft,
                        Anchor = Anchor.CentreLeft,
                        Shear = new Vector2(-SHEAR_AMOUNT, 0),
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Shear = new Vector2(-SHEAR_AMOUNT, 0),
                        Spacing = new Vector2(-6),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = Text,
                                WebFontSize = 20,
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = Description,
                                WebFontSize = 14,
                                Alpha = .8f,
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };
    }
}
