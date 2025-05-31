using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;
using osu.Framework.Allocation;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ConfirmExitPanel : ButtonPanel
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    public ConfirmExitPanel()
    {
        Icon = FontAwesome6.Solid.DoorOpen;
        Text = LocalizationStrings.General.PanelConfirmExit;
        Buttons = new ButtonData[]
        {
            new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, () => game.Exit()),
            new CancelButtonData()
        };
    }
}
