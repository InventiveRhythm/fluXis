using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Graphics.Menu;

public partial class FluXisMenu : osu.Framework.Graphics.UserInterface.Menu
{
    public FluXisMenu(Direction direction, bool topLevelMenu = false)
        : base(direction, topLevelMenu)
    {
        BackgroundColour = FluXisColors.Surface2;
        MaskingContainer.CornerRadius = 5;
        ItemsContainer.Padding = new MarginPadding(5);
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() =>
        new FluXisMenu(Direction.Vertical) { Anchor = Direction == Direction.Horizontal ? Anchor.TopLeft : Anchor.TopRight };

    protected override void UpdateSize(Vector2 newSize)
    {
        if (Direction == Direction.Vertical)
        {
            Width = newSize.X + 30;
            this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
        }
        else
        {
            Height = newSize.Y;
            this.ResizeWidthTo(newSize.X + 30, 300, Easing.OutQuint);
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
        return new DrawableFluXisMenuItem(item);
    }

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new BasicScrollContainer(direction) { ScrollbarVisible = false };
}
