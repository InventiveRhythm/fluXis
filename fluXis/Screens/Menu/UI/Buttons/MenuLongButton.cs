using fluXis.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Menu.UI.Buttons;

/// <summary>
/// A button with an icon and text.
/// </summary>
public partial class MenuLongButton : MenuButtonBase
{
    protected override Drawable[] CreateContent()
    {
        return new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(20),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 32,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 32),
                        new Dimension(GridSizeMode.Absolute, 12),
                        new Dimension(),
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new FluXisSpriteIcon
                            {
                                Icon = Icon,
                                Size = new Vector2(32),
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                                Shear = new Vector2(-SHEAR_AMOUNT, 0),
                            },
                            Empty(),
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Shear = new Vector2(-SHEAR_AMOUNT, 0),
                                Spacing = new Vector2(-6),
                                Children = new Drawable[]
                                {
                                    new TruncatingText
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Text = Text,
                                        WebFontSize = 20,
                                        Origin = Anchor.CentreLeft,
                                        Anchor = Anchor.CentreLeft
                                    },
                                    new TruncatingText
                                    {
                                        RelativeSizeAxes = Axes.X,
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
                }
            }
        };
    }
}
