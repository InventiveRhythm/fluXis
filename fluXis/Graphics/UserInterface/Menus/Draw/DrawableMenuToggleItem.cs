using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus.Items;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.UserInterface.Menus.Draw;

public partial class DrawableMenuToggleItem : DrawableFluXisMenuItem<MenuToggleItem>
{
    public DrawableMenuToggleItem(MenuToggleItem item)
        : base(item)
    {
    }

    protected override Drawable CreateRightContent()
    {
        if (!Item.IsActive)
            return base.CreateRightContent();

        return new Container
        {
            Width = 14 * 2 + 16,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding(14),
            Child = new FluXisSpriteIcon
            {
                Size = new Vector2(16),
                Icon = FontAwesome6.Solid.Check
            }
        };
    }
}
