using System;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Localization;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Buttons.Presets;

public class PrimaryButtonData : ButtonData
{
    public PrimaryButtonData(LocalisableString text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Accent2;
        TextColor = FluXisColors.Background1;
        HoldToConfirm = holdToConfirm;
    }

    public PrimaryButtonData(Action action, bool holdToConfirm = false)
        : this(LocalizationStrings.General.PanelGenericConfirm, action, holdToConfirm)
    {
    }
}
