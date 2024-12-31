using System;
using fluXis.Graphics.UserInterface.Color;

namespace fluXis.Graphics.UserInterface.Buttons.Presets;

public class SecondaryButtonData : ButtonData
{
    public SecondaryButtonData(string text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Background4;
        TextColor = FluXisColors.Text;
        HoldToConfirm = holdToConfirm;
    }
}
