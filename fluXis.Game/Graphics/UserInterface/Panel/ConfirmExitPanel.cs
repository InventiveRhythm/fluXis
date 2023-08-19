using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ConfirmExitPanel : ButtonPanel
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    public ConfirmExitPanel()
    {
        Text = "Are you sure you want to exit the game?";
        Buttons = new ButtonData[]
        {
            new()
            {
                Text = "Yes",
                Color = FluXisColors.ButtonRed,
                Action = () => game.Exit()
            },
            new()
            {
                Text = "No",
                Action = Hide
            }
        };
    }
}
