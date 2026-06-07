using fluXis.Graphics.Sprites.Icons;

namespace fluXis.Graphics.UserInterface.Menus.Items;

public class MenuSpacerItem : FluXisMenuItem
{
    public MenuSpacerItem()
        : base(" ", Phosphor.Bold.Circle, MenuItemType.Normal, () => { })
    {
    }
}
