using System;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Menu;

public class FluXisMenuItem : MenuItem
{
    public readonly MenuItemType Type;

    public FluXisMenuItem(LocalisableString text, MenuItemType type = MenuItemType.Normal)
        : this(text, type, null)
    {
    }

    public FluXisMenuItem(LocalisableString text, MenuItemType type, Action action)
        : base(text, action)
    {
        Type = type;
    }
}
