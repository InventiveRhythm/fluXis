using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Menus.Items;

#nullable enable

/// <summary>
/// a menu item that toggles on/off
/// </summary>
public class MenuToggleItem : FluXisMenuItem
{
    public bool IsActive => bindable?.Value ?? func?.Invoke() ?? false;

    private Bindable<bool>? bindable { get; }
    private Func<bool>? func { get; }

    public MenuToggleItem(LocalisableString text, IconUsage icon, Bindable<bool> bind)
        : base(text, icon, MenuItemType.Normal, () => bind.Value = !bind.Value)
    {
        bindable = bind;
    }

    public MenuToggleItem(LocalisableString text, IconUsage icon, Action action, Func<bool> active)
        : base(text, icon, MenuItemType.Normal, action)
    {
        func = active;
    }
}
