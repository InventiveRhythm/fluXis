using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

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

            case JoystickButton.Button2: // A
                Accept();
                return true;

            case JoystickButton.Button3: // B
                this.Exit();
                return true;

            case JoystickButton.Button4: // Y
                RandomMap();
                return true;

            case JoystickButton.GamePadLeftShoulder: // LB
                return true;

            case JoystickButton.GamePadRightShoulder: // RB
                return true;

            case JoystickButton.Axis2Positive: // LeftStick Up
                changeLetter(-1);
                return true;

            case JoystickButton.Axis2Negative: // LeftStick Down
                changeLetter(1);
                return true;

            case JoystickButton.Hat1Up: // Up
                changeSubItemSelection(-1);
                return true;

            case JoystickButton.Hat1Down: // Down
                changeSubItemSelection(1);
                return true;

            case JoystickButton.Hat1Left: // Left
                changeItemSelection(-1);
                return true;

            case JoystickButton.Hat1Right: // Right
                changeItemSelection(1);
                return true;

            case JoystickButton.Button10: // Start
                footer.OpenSettings();
                return true;
        }

        return false;
    }
}
