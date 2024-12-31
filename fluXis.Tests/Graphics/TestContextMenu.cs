using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Menus;
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
            new FluXisMenuItem("Make Gray", () =>
            {
                Colour = Colour4.Gray;
            }),
            new FluXisMenuItem("Make Blue", MenuItemType.Highlighted, () =>
            {
                Colour = Colour4.Blue;
            }),
            new FluXisMenuItem("Make Red", MenuItemType.Dangerous, () =>
            {
                Colour = Colour4.Red;
            })
        };
    }
}
