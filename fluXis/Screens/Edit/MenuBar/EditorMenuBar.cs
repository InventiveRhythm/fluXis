using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Screens.Edit.MenuBar;

public partial class EditorMenuBar : FluXisMenu
{
    public EditorMenuBar()
        : base(Direction.Horizontal, true)
    {
        RelativeSizeAxes = Axes.X;
        BackgroundColour = FluXisColors.Background1;
        // ItemsContainer = new Vector2(10, 0);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        MaskingContainer.CornerRadius = 0;
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowMediumNoOffset;
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        newSize.Y = 45;
        base.UpdateSize(newSize);
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
    {
        if (item is FluXisMenuItem i)
            return new DrawableFluXisMenuItem(i, false);

        return base.CreateDrawableMenuItem(item);
    }
}
