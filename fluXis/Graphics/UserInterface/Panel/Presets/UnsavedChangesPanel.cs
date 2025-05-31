using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using JetBrains.Annotations;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class UnsavedChangesPanel : ButtonPanel
{
    public UnsavedChangesPanel(Action save, Action exit, [CanBeNull] Action cancel = null)
    {
        Icon = FontAwesome6.Solid.ExclamationTriangle;
        Text = "There are unsaved changes.";
        SubText = "Are you sure you want to exit?";
        IsDangerous = true;
        Buttons = new ButtonData[]
        {
            new PrimaryButtonData("Save and exit.", save),
            new DangerButtonData("Exit without saving.", exit),
            new CancelButtonData("Nevermind, back to editing.", cancel)
        };
    }
}
