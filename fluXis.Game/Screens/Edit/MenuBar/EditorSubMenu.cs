using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.MenuBar;

public partial class EditorSubMenu : FluXisMenu
{
    public EditorSubMenu()
        : base(Direction.Vertical)
    {
        BackgroundColour = FluXisColors.Background2;
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();
    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableFluXisMenuItem(item);
}
