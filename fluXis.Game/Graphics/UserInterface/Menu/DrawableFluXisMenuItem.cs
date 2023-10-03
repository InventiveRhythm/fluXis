using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Menu;

public partial class DrawableFluXisMenuItem : osu.Framework.Graphics.UserInterface.Menu.DrawableMenuItem
{
    protected virtual float TextSize => 20;
    public bool ShowChevron { get; set; } = true;

    private bool shouldShowChevron => Item.Items.Count > 0 && ShowChevron;
    private bool shouldShowCheck => (Item as FluXisMenuItem)?.IsActive?.Invoke() ?? false;
    private bool isSpacer => Item is FluXisMenuSpacer;

    private const int content_size = 20;
    private const int icon_size = 16;
    private const float icon_margin = (content_size - icon_size) / 2f;

    [Resolved]
    private UISamples samples { get; set; }

    private FluXisSpriteText text;
    private SpriteIcon icon;

    public DrawableFluXisMenuItem(MenuItem item)
        : base(item)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        CornerRadius = 5;
        Masking = true;
        Foreground.Anchor = Anchor.CentreLeft;
        Foreground.Origin = Anchor.CentreLeft;

        if (isSpacer)
        {
            BackgroundColour = Colour4.Transparent;
            BackgroundColourHover = Colour4.Transparent;
            Foreground.AutoSizeAxes = Axes.Y;
            Foreground.RelativeSizeAxes = Axes.X;
            return;
        }

        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = Colour4.White.Opacity(.2f);

        AddRangeInternal(new[]
        {
            new SpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronRight,
                Alpha = shouldShowChevron ? 1f : 0f,
                Size = new Vector2(icon_size * .8f),
                Margin = new MarginPadding(icon_margin) { Right = icon_margin * 4 },
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Shadow = true
            },
            new SpriteIcon
            {
                Icon = FontAwesome.Solid.Check,
                Alpha = shouldShowCheck ? 1f : 0f,
                Size = new Vector2(icon_size * .8f),
                Margin = new MarginPadding(icon_margin) { Right = icon_margin * 4 },
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Shadow = true
            }
        });

        updateColor();
    }

    private void updateColor()
    {
        text.Colour = icon.Colour = (Item as FluXisMenuItem)?.Type switch
        {
            MenuItemType.Highlighted => FluXisColors.GetThemeColor(.8f, .8f),
            MenuItemType.Dangerous => FluXisColors.Red,
            _ => FluXisColors.Text
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!isSpacer)
            samples.Hover();

        return base.OnHover(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Item is FluXisMenuItem item && (!item.Enabled?.Invoke() ?? false)) return true;

        if (!isSpacer)
            samples.Click();

        return base.OnClick(e);
    }

    protected override Drawable CreateContent()
    {
        if (isSpacer)
        {
            return new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Vertical = 5 },
                Child = new CircularContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 4,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background5
                        }
                    }
                }
            };
        }

        return new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Alpha = (Item as FluXisMenuItem)?.Enabled?.Invoke() ?? true ? 1f : .75f,
            Spacing = new Vector2(5),
            Padding = new MarginPadding { Horizontal = 5, Vertical = 5 },
            Children = new Drawable[]
            {
                icon = new SpriteIcon
                {
                    Icon = (Item as FluXisMenuItem)?.Icon ?? FontAwesome.Solid.Circle,
                    Size = new Vector2(icon_size),
                    Margin = new MarginPadding(icon_margin),
                    Shadow = true,
                },
                text = new FluXisSpriteText
                {
                    FontSize = TextSize,
                    Text = Item.Text.ToString(),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shadow = true
                }
            }
        };
    }
}
