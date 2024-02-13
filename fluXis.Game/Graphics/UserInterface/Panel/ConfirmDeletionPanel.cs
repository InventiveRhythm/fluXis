using System;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ConfirmDeletionPanel : ButtonPanel
{
    public ConfirmDeletionPanel(Action confirm, Action cancel = null, string itemName = "")
    {
        Text = $"Are you sure you want to delete this{(string.IsNullOrEmpty(itemName) ? "" : $" {itemName}")}?";
        SubText = "This action cannot be undone.";
        ButtonWidth = 200;
        Buttons = new ButtonData[]
        {
            new()
            {
                Text = "Yes delete it.",
                Color = FluXisColors.ButtonRed,
                HoldToConfirm = true,
                Action = confirm
            },
            new()
            {
                Text = "No nevermind.",
                Action = cancel ?? (() => { })
            }
        };
    }
}
