using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Menus.Items;

/// <summary>
/// a menu item with more items
/// </summary>
public class MenuExpandItem : FluXisMenuItem
{
    public bool ShowIcon { get; set; } = true;

    public MenuExpandItem(LocalisableString text, IconUsage icon, IEnumerable<MenuItem> items)
        : base(text, icon, MenuItemType.Normal, () => { })
    {
        Items = items.ToArray();
    }
}
