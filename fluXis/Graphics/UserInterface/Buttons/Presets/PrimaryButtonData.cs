using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Buttons.Presets;

public class PrimaryButtonData : ButtonData
{
    public PrimaryButtonData(LocalisableString text, Action action, bool holdToConfirm = false)
    {
        Text = text;
        Action = action;
        Color = Theme.Primary;
        TextColor = Theme.Background1;
        HoldToConfirm = holdToConfirm;
    }

    public PrimaryButtonData(Action action, bool holdToConfirm = false)
        : this(LocalizationStrings.General.PanelGenericConfirm, action, holdToConfirm)
    {
    }
}
