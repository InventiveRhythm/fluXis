using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Menu.UI.Buttons;

/// <summary>
/// A button with an icon, text and a background image.
/// </summary>
public partial class MenuImageButton : MenuButtonBase
{
    public override LocalisableString Description
    {
        get => description;
        set
        {
            description = value;

            if (descriptionText != null)
                descriptionText.Text = value;
        }
    }

    public SpriteStack<Drawable> Stack { get; private set; }
    public Drawable DefaultSprite { get; init; }

    private LocalisableString description;
    private FluXisSpriteText descriptionText;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (DefaultSprite != null)
            Stack.Add(DefaultSprite);
    }

    protected override Drawable[] CreateContent()
    {
        return new Drawable[]
        {
            Stack = new SpriteStack<Drawable>
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(1.1f),
                Shear = new Vector2(-SHEAR_AMOUNT, 0)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Height = .8f,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Colour = ColourInfo.GradientVertical(FluXisColors.Background2.Opacity(0), FluXisColors.Background2),
                Alpha = .8f
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Height = .2f,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Colour = FluXisColors.Background2,
                Alpha = .8f
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(20),
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
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
                                    descriptionText = new TruncatingText
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
