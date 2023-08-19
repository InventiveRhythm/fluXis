using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Context;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Tests.Graphics;

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
        public MenuItem[] ContextMenuItems
        {
            get
            {
                List<MenuItem> items = new()
                {
                    new MenuItem("Make Gray", () =>
                    {
                        Colour = Colour4.Gray;
                    }),
                    new MenuItem("Make Blue", () =>
                    {
                        Colour = Colour4.Blue;
                    }),
                    new MenuItem("Make Red", () =>
                    {
                        Colour = Colour4.Red;
                    })
                };

                return items.ToArray();
            }
        }
    }
}
