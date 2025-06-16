using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Menus;

public abstract class FluXisMenuItem : MenuItem
{
    public MenuItemType Type { get; }
    public IconUsage Icon { get; }

    public Func<bool> IsEnabled { get; init; } = () => true;

    protected FluXisMenuItem(LocalisableString text, IconUsage icon, MenuItemType type, Action action)
        : base(text, action)
    {
        Type = type;
        Icon = icon;
    }
}
