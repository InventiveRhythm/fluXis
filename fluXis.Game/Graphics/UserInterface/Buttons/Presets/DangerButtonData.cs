using System;
using fluXis.Game.Graphics.UserInterface.Color;

namespace fluXis.Game.Graphics.UserInterface.Buttons.Presets;

public class DangerButtonData : ButtonData
{
    public DangerButtonData(string text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Red;
        TextColor = FluXisColors.Background1;
        HoldToConfirm = holdToConfirm;
    }
}
