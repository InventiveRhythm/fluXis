using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.MenuBar;

public partial class EditorSubMenu : FluXisMenu
{
    public EditorSubMenu()
        : base(Direction.Vertical)
    {
        BackgroundColour = FluXisColors.Background2;
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();
}
