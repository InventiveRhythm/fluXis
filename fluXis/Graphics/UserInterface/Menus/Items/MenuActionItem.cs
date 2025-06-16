using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Menus.Items;

/// <summary>
/// a menu item that executes an action
/// </summary>
public class MenuActionItem : FluXisMenuItem
{
    public MenuActionItem(LocalisableString text, IconUsage icon, Action action)
        : this(text, icon, MenuItemType.Normal, action)
    {
    }

    public MenuActionItem(LocalisableString text, IconUsage icon, MenuItemType type, Action action)
        : base(text, icon, type, action)
    {
    }
}
