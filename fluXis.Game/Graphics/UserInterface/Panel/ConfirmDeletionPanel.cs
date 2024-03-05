using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ConfirmDeletionPanel : ButtonPanel
{
    public ConfirmDeletionPanel(Action confirm, Action cancel = null, string itemName = "")
    {
        Icon = FontAwesome6.Solid.Trash;
        Text = $"Are you sure you want to delete this{(string.IsNullOrEmpty(itemName) ? "" : $" {itemName}")}?";
        SubText = "This action cannot be undone.";
        Buttons = new ButtonData[]
        {
            new DangerButtonData(COMMON_CONFIRM, confirm, true),
            new CancelButtonData(COMMON_CANCEL, cancel)
        };
    }
}
