using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Localization;
using osu.Framework.Allocation;

namespace fluXis.Game.Graphics.UserInterface.Panel;

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
