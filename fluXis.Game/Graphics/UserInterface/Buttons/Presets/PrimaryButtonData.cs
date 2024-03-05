using System;
using fluXis.Game.Graphics.UserInterface.Color;

namespace fluXis.Game.Graphics.UserInterface.Buttons.Presets;

public class PrimaryButtonData : ButtonData
{
    public PrimaryButtonData(string text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Accent2;
        TextColor = FluXisColors.Background1;
        HoldToConfirm = holdToConfirm;
    }
}
