using System;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Input;

public partial class GamepadHandler : Component
{
    public override bool HandlePositionalInput => true;
    public override bool HandleNonPositionalInput => true;

    public static event Action<bool> OnGamepadStatusChanged;
    public static bool GamepadConnected { get; private set; }

    public GamepadHandler()
    {
        RelativeSizeAxes = Axes.Both;
    }

    private static bool updateStatus(bool status)
    {
        if (GamepadConnected == status) return false;

        GamepadConnected = status;
        OnGamepadStatusChanged?.Invoke(status);
        return true;
    }

    protected override bool Handle(UIEvent e) => updateStatus(e is JoystickEvent);
}
