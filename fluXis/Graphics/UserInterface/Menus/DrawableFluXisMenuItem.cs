using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Menus;

public partial class DrawableFluXisMenuItem : Menu.DrawableMenuItem
{
    protected new FluXisMenuItem Item => base.Item as FluXisMenuItem;

    private bool showChevron { get; }
    private bool shouldShowChevron => Item.Items.Count > 0 && showChevron;
    private bool shouldShowCheck => Item.IsActive?.Invoke() ?? false;
    private bool isEnabled => Item.Enabled?.Invoke() ?? true;

    [Resolved]
    private UISamples samples { get; set; }

    private FluXisSpriteText text;
    private SpriteIcon icon;

    public DrawableFluXisMenuItem(FluXisMenuItem item, bool showChevron = true)
        : base(item)
    {
        this.showChevron = showChevron;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Foreground.Anchor = Anchor.CentreLeft;
        Foreground.Origin = Anchor.CentreLeft;

        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = FluXisColors.Text.Opacity(.2f);

        AddInternal(getRightContainer().With(d =>
        {
            d.Anchor = Anchor.CentreRight;
            d.Origin = Anchor.CentreRight;
        }));

        updateColor();
    }

    private void updateColor()
    {
        text.Colour = icon.Colour = Item.Type switch
        {
            MenuItemType.Highlighted => FluXisColors.GetThemeColor(.8f, .8f),
            MenuItemType.Dangerous => FluXisColors.Red,
            _ => FluXisColors.Text
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        return base.OnHover(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click(!isEnabled);
        return isEnabled && base.OnClick(e);
    }

    protected override Drawable CreateContent()
    {
        return new GridContainer
        {
            AutoSizeAxes = Axes.Both,
            Alpha = isEnabled ? 1f : .6f,
            RowDimensions = new Dimension[]
            {
                new(GridSizeMode.AutoSize)
            },
            ColumnDimensions = new Dimension[]
            {
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Padding = new MarginPadding(12),
                        Child = icon = new FluXisSpriteIcon
                        {
                            Size = new Vector2(20),
                            Icon = Item.Icon
                        }
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Child = text = new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 16,
                            Text = Item.Text.Value
                        }
                    },
                    new Container { Width = getRightSize() }
                }
            }
        };
    }

    private float getRightSize()
    {
        var num = 14;

        if (getRightContent() != null)
            num += 30;

        return num;
    }

    private Container getRightContainer()
    {
        var drawable = getRightContent();

        if (drawable == null)
        {
            return new Container
            {
                AutoSizeAxes = Axes.X,
                Child = Empty(),
                Padding = new MarginPadding { Right = 12 }
            };
        }

        return new Container
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Padding = new MarginPadding { Left = 14, Right = 14 },
            Child = drawable.With(d => d.Anchor = d.Origin = Anchor.CentreRight)
        };
    }

    private Drawable getRightContent()
    {
        if (shouldShowChevron)
        {
            return new FluXisSpriteIcon
            {
                Size = new Vector2(16),
                Icon = FontAwesome6.Solid.AngleRight
            };
        }

        if (shouldShowCheck)
        {
            return new FluXisSpriteIcon
            {
                Size = new Vector2(16),
                Icon = FontAwesome6.Solid.Check
            };
        }

        return null;
    }
}
