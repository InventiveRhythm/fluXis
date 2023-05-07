using fluXis.Game.Graphics.Menu;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Context;

public partial class FluXisContextMenu : FluXisMenu
{
    public FluXisContextMenu()
        : base(Direction.Vertical)
    {
        MaxHeight = 300;
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new FluXisContextMenu();
}
