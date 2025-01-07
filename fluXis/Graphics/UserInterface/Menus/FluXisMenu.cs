using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Graphics.UserInterface.Menus;

public partial class FluXisMenu : Menu
{
    public FluXisMenu(Direction direction, bool topLevelMenu = false)
        : base(direction, topLevelMenu)
    {
        BackgroundColour = FluXisColors.Background4;
    }

    protected override Menu CreateSubMenu() =>
        new FluXisMenu(Direction.Vertical) { Anchor = Direction == Direction.Horizontal ? Anchor.TopLeft : Anchor.TopRight };

    protected override void UpdateSize(Vector2 newSize)
    {
        if (Direction == Direction.Vertical)
        {
            Width = newSize.X;
            this.ResizeHeightTo(newSize.Y, 400, Easing.OutQuint);
        }
        else
        {
            Height = newSize.Y;
            this.ResizeWidthTo(newSize.X, 400, Easing.OutQuint);
        }
    }

    protected override void AnimateClose()
    {
        this.FadeOut(200);
    }

    protected override void AnimateOpen()
    {
        this.FadeIn(200);
    }

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
    {
        return item switch
        {
            FluXisMenuSpacer spacer => new DrawableFluXisMenuSpacer(spacer),
            FluXisMenuItem menuItem => new DrawableFluXisMenuItem(menuItem),
            _ => new BasicMenu.BasicDrawableMenuItem(item)
        };
    }

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new FluXisScrollContainer(direction) { ScrollbarVisible = false };
}
