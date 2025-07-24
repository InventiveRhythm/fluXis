using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Buttons.Presets;

public class CancelButtonData : ButtonData
{
    public CancelButtonData(LocalisableString text, Action action = null)
    {
        Text = text;
        Action = action;
        Color = Theme.Background2;
        TextColor = Theme.Text;
    }

    public CancelButtonData(Action action = null)
        : this(LocalizationStrings.General.PanelGenericCancel, action)
    {
    }
}
