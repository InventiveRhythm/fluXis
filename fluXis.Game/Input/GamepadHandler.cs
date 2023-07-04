using System;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Logging;

namespace fluXis.Game.Input;

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
        Logger.Log($"Gamepad status changed to {status}", LoggingTarget.Runtime, LogLevel.Important);
        return true;
    }

    protected override bool Handle(UIEvent e)
    {
        return updateStatus(e is JoystickEvent);
    }
}
