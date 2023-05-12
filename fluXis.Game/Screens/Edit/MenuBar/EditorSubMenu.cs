using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Menu;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.MenuBar;

public partial class EditorSubMenu : FluXisMenu
{
    public EditorSubMenu()
        : base(Direction.Vertical)
    {
        ItemsContainer.Padding = new MarginPadding();
        BackgroundColour = FluXisColors.Background2;
        MaskingContainer.CornerRadius = 0;
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorSubMenu();
    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorSubMenuItem(item);
}
