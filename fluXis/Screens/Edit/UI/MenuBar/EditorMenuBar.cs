using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Screens.Edit.UI.MenuBar;

public partial class EditorMenuBar : FluXisMenu
{
    public EditorMenuBar()
        : base(Direction.Horizontal, true)
    {
        RelativeSizeAxes = Axes.X;
        BackgroundColour = Theme.Background1;
        // ItemsContainer = new Vector2(10, 0);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        MaskingContainer.CornerRadius = 0;
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        newSize.Y = 45;
        base.UpdateSize(newSize);
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
    {
        if (item is MenuExpandItem i)
            i.ShowIcon = false;

        return base.CreateDrawableMenuItem(item);
    }

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction)
    {
        var scroll = base.CreateScrollContainer(direction);
        scroll.ClampExtension = 0;
        return scroll;
    }
}
