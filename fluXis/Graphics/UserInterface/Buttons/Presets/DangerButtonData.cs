using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Buttons.Presets;

public class DangerButtonData : ButtonData
{
    public DangerButtonData(LocalisableString text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = Theme.Red;
        TextColor = Theme.Background1;
        HoldToConfirm = holdToConfirm;
    }
}
