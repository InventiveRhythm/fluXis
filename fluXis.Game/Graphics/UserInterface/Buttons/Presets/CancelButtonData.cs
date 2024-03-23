using System;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;

namespace fluXis.Game.Graphics.UserInterface.Buttons.Presets;

public class CancelButtonData : ButtonData
{
    public CancelButtonData(string text = ButtonPanel.COMMON_CANCEL, Action action = null)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Background2;
        TextColor = FluXisColors.Text;
    }
}
