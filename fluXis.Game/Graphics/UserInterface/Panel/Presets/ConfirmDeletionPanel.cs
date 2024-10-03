using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Panel.Types;
using fluXis.Game.Localization;

namespace fluXis.Game.Graphics.UserInterface.Panel.Presets;

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
