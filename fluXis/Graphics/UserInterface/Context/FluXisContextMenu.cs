using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Graphics.UserInterface.Context;

public partial class FluXisContextMenu : FluXisMenu
{
    public FluXisContextMenu()
        : base(Direction.Vertical)
    {
        MaskingContainer.EdgeEffect = Styling.ShadowSmall;
        MaskingContainer.CornerRadius = 10;
        ItemsContainer.Masking = true;
        ItemsContainer.CornerRadius = 10;
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        if (newSize.Y == 0)
            this.ResizeTo(0, 400, Easing.OutQuint);
        else
            this.ResizeTo(newSize, 400, Easing.OutQuint);
    }

    protected override Menu CreateSubMenu() => new FluXisContextMenu();
}
