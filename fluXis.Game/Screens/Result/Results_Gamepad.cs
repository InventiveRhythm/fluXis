using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Result;

public partial class ResultsScreen
{
    protected override bool OnJoystickPress(JoystickPressEvent e) => handleGeneric(e);

    private bool handleGeneric(JoystickPressEvent e)
    {
        switch (e.Button)
        {
            case JoystickButton.Button3: // B
                this.Exit();
                return true;
        }

        return false;
    }
}
