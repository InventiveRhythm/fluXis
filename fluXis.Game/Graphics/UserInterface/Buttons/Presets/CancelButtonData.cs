using System;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Localization;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Buttons.Presets;

public class CancelButtonData : ButtonData
{
    public CancelButtonData(LocalisableString text, Action action = null)
    {
        Text = text;
        Action = action;
        Color = FluXisColors.Background2;
        TextColor = FluXisColors.Text;
    }

    public CancelButtonData(Action action = null)
        : this(LocalizationStrings.General.PanelGenericCancel, action)
    {
    }
}
