using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.UserInterface.Context;

public partial class FluXisContextMenu : FluXisMenu
{
    public FluXisContextMenu()
        : base(Direction.Vertical)
    {
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new FluXisContextMenu();
}
