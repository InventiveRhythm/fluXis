using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Menus.Draw;

public abstract partial class DrawableFluXisMenuItem<T> : Menu.DrawableMenuItem
    where T : FluXisMenuItem
{
    protected new T Item => base.Item as T;
    private bool isEnabled => Item.IsEnabled?.Invoke() ?? true;

    [Resolved]
    private UISamples samples { get; set; }

    protected DrawableFluXisMenuItem(T item)
        : base(item)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Foreground.Anchor = Anchor.CentreLeft;
        Foreground.Origin = Anchor.CentreLeft;

        BackgroundColour = Colour4.Transparent;
        BackgroundColourHover = Theme.Text.Opacity(.2f);

        ForegroundColour = ForegroundColourHover = Item.Type switch
        {
            MenuItemType.Highlighted => Theme.Highlight,
            MenuItemType.Dangerous => Theme.Red,
            _ => Colour4.White
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
        var right = CreateRightContent();
        Schedule(() => AddInternal(right.With(x => x.Anchor = x.Origin = Anchor.CentreRight)));

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
                new(minSize: right.Width)
            },
            Content = new[]
            {
                new[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Padding = new MarginPadding(12),
                        Child = new FluXisSpriteIcon
                        {
                            Size = new Vector2(20),
                            Icon = Item.Icon
                        }
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Child = new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 16,
                            Text = Item.Text.Value
                        }
                    },
                    Empty().With(x => x.Width = right.Width)
                }
            }
        };
    }

    protected virtual Drawable CreateRightContent() => Empty().With(x => x.Width = 12);
}
