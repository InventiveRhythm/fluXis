using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using osu.Framework.Graphics;

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
}
