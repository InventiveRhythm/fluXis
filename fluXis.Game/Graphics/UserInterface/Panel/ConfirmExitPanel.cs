using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using osu.Framework.Allocation;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ConfirmExitPanel : ButtonPanel
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    public ConfirmExitPanel()
    {
        Icon = FontAwesome6.Solid.DoorOpen;
        Text = "Are you sure you want to exit?";
        Buttons = new ButtonData[]
        {
            new DangerButtonData(COMMON_CONFIRM, () => game.Exit()),
            new CancelButtonData(COMMON_CANCEL)
        };
    }
}
