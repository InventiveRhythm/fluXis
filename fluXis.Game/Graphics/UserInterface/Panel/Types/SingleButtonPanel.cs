using System;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.UserInterface.Panel.Types;

public partial class SingleButtonPanel : ButtonPanel
{
    public SingleButtonPanel(IconUsage icon, string title, string sub, string button = "Okay.", Action action = null)
    {
        Icon = icon;
        Text = title;
        SubText = sub;
        Buttons = new ButtonData[]
        {
            new CancelButtonData(button, action)
        };
    }
}
