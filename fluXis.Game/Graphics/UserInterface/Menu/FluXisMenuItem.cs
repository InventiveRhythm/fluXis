using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Menu;

public class FluXisMenuItem : MenuItem
{
    public MenuItemType Type { get; }
    public IconUsage Icon { get; } = FontAwesome.Solid.Circle;
    public bool Enabled { get; set; } = true;
    public Func<bool> IsActive { get; set; }

    public FluXisMenuItem(LocalisableString text)
        : this(text, MenuItemType.Normal, () => { })
    {
    }

    public FluXisMenuItem(LocalisableString text, IconUsage icon)
        : this(text, icon, MenuItemType.Normal, () => { })
    {
    }

    public FluXisMenuItem(LocalisableString text, Action action)
        : this(text, MenuItemType.Normal, action)
    {
    }

    public FluXisMenuItem(LocalisableString text, IconUsage icon, Action action)
        : this(text, icon, MenuItemType.Normal, action)
    {
    }

    public FluXisMenuItem(LocalisableString text, IconUsage icon, MenuItemType type, Action action)
        : this(text, type, action)
    {
        Icon = icon;
    }

    public FluXisMenuItem(LocalisableString text, MenuItemType type, Action action)
        : base(text, action)
    {
        Type = type;
    }
}
