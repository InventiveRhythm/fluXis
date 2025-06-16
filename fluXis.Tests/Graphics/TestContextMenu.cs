using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Tests.Graphics;

public partial class TestContextMenu : FluXisTestScene
{
    public TestContextMenu()
    {
        Add(new FluXisContextMenuContainer
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new ContextBox
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Gray
                }
            }
        });
    }

    private partial class ContextBox : Box, IHasContextMenu
    {
        public MenuItem[] ContextMenuItems => new MenuItem[]
        {
            new MenuActionItem("Make Gray", FontAwesome6.Solid.Circle, () => Colour = Colour4.Gray),
            new MenuActionItem("Make Blue", FontAwesome6.Solid.Circle, MenuItemType.Highlighted, () => Colour = Colour4.Blue),
            new MenuActionItem("Make Red", FontAwesome6.Solid.Circle, MenuItemType.Dangerous, () => Colour = Colour4.Red)
        };
    }
}
