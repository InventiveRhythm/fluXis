using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Graphics.UserInterface.Context;

public partial class FluXisContextMenuContainer : ContextMenuContainer
{
    protected override Menu CreateMenu() => new FluXisContextMenu();
}
