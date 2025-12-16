using osu.Framework.Input;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Select;

public partial class SelectScreen
{
    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        switch (e.Button)
        {
            case JoystickButton.Button1: // X
                modsOverlay.ToggleVisibility();
                return true;

            case JoystickButton.Button4: // Y
                RandomMap();
                return true;

            case JoystickButton.GamePadLeftShoulder: // LB
                return true;

            case JoystickButton.GamePadRightShoulder: // RB
                return true;

            case JoystickButton.Axis5Positive: // RightStick Down
                changeLetter(1);
                return true;

            case JoystickButton.Axis5Negative: // RightStick Up
                changeLetter(-1);
                return true;

            case JoystickButton.Button10: // Start
                footer.OpenSettings();
                return true;
        }

        return false;
    }
}
