using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Screens.Edit.MenuBar;

public partial class EditorMenuBar : FluXisMenu
{
    public EditorMenuBar()
        : base(Direction.Horizontal, true)
    {
        RelativeSizeAxes = Axes.X;
        MaskingContainer.CornerRadius = 0;
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowMediumNoOffset;
        BackgroundColour = FluXisColors.Background1;
        // ItemsContainer = new Vector2(10, 0);
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        newSize.Y = 45;
        base.UpdateSize(newSize);
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();
    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableFluXisMenuItem(item) { ShowChevron = false };
}
