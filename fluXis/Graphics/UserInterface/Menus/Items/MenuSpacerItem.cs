using fluXis.Graphics.Sprites.Icons;

namespace fluXis.Graphics.UserInterface.Menus.Items;

public class MenuSpacerItem : FluXisMenuItem
{
    public MenuSpacerItem()
        : base(" ", FontAwesome6.Solid.Circle, MenuItemType.Normal, () => { })
    {
    }
}
