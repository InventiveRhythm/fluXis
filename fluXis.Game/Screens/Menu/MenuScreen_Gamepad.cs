using osu.Framework.Input;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen
{
    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        if (canPlayAnimation()) return true;

        switch (e.Button)
        {
            case JoystickButton.Button1: // X
                continueToRankings();
                return true;

            case JoystickButton.Button2: // A
                continueToPlay();
                return true;

            case JoystickButton.Button3: // B
                continueToMultiplayer();
                return true;

            case JoystickButton.Button4: // Y
                continueToBrowse();
                return true;

            case JoystickButton.Button9: // Back
                Game.Exit();
                return true;

            case JoystickButton.Button10: // Start
                settings.ToggleVisibility();
                return true;
        }

        return false;
    }
}
