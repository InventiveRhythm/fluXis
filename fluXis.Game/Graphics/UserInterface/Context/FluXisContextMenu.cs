using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Context;

public partial class FluXisContextMenu : FluXisMenu
{
    public FluXisContextMenu()
        : base(Direction.Vertical)
    {
        MaskingContainer.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        if (newSize.Y == 0)
            this.ScaleTo(.8f, 400, Easing.OutQuint);
        else
        {
            Size = newSize;
            Width = newSize.X + 30;
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new FluXisContextMenu();
}
