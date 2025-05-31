using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ConfirmDeletionPanel : ButtonPanel
{
    public ConfirmDeletionPanel(Action confirm, Action cancel = null, string itemName = "")
    {
        Icon = FontAwesome6.Solid.Trash;
        Text = $"Are you sure you want to delete this{(string.IsNullOrEmpty(itemName) ? "" : $" {itemName}")}?";
        SubText = "This action cannot be undone.";
        IsDangerous = true;
        Buttons = new ButtonData[]
        {
            new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, confirm, true),
            new CancelButtonData(cancel)
        };
    }
}
