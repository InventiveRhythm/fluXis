using System;
using fluXis.Game.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Input;

public partial class GameplayInput : Drawable, IKeyBindingHandler<FluXisGameplayKeybind>
{
    private static readonly FluXisGameplayKeybind[] keys1 =
    {
        FluXisGameplayKeybind.Key1k1
    };

    private static readonly FluXisGameplayKeybind[] keys2 =
    {
        FluXisGameplayKeybind.Key2k1,
        FluXisGameplayKeybind.Key2k2
    };

    private static readonly FluXisGameplayKeybind[] keys3 =
    {
        FluXisGameplayKeybind.Key3k1,
        FluXisGameplayKeybind.Key3k2,
        FluXisGameplayKeybind.Key3k3
    };

    private static readonly FluXisGameplayKeybind[] keys4 =
    {
        FluXisGameplayKeybind.Key4k1,
        FluXisGameplayKeybind.Key4k2,
        FluXisGameplayKeybind.Key4k3,
        FluXisGameplayKeybind.Key4k4
    };

    private static readonly FluXisGameplayKeybind[] keys5 =
    {
        FluXisGameplayKeybind.Key5k1,
        FluXisGameplayKeybind.Key5k2,
        FluXisGameplayKeybind.Key5k3,
        FluXisGameplayKeybind.Key5k4,
        FluXisGameplayKeybind.Key5k5
    };

    private static readonly FluXisGameplayKeybind[] keys6 =
    {
        FluXisGameplayKeybind.Key6k1,
        FluXisGameplayKeybind.Key6k2,
        FluXisGameplayKeybind.Key6k3,
        FluXisGameplayKeybind.Key6k4,
        FluXisGameplayKeybind.Key6k5,
        FluXisGameplayKeybind.Key6k6
    };

    private static readonly FluXisGameplayKeybind[] keys7 =
    {
        FluXisGameplayKeybind.Key7k1,
        FluXisGameplayKeybind.Key7k2,
        FluXisGameplayKeybind.Key7k3,
        FluXisGameplayKeybind.Key7k4,
        FluXisGameplayKeybind.Key7k5,
        FluXisGameplayKeybind.Key7k6,
        FluXisGameplayKeybind.Key7k7
    };

    private static readonly FluXisGameplayKeybind[] keys8 =
    {
        FluXisGameplayKeybind.Key8k1,
        FluXisGameplayKeybind.Key8k2,
        FluXisGameplayKeybind.Key8k3,
        FluXisGameplayKeybind.Key8k4,
        FluXisGameplayKeybind.Key8k5,
        FluXisGameplayKeybind.Key8k6,
        FluXisGameplayKeybind.Key8k7,
        FluXisGameplayKeybind.Key8k8
    };

    private static readonly FluXisGameplayKeybind[] keys9 =
    {
        FluXisGameplayKeybind.Key9k1,
        FluXisGameplayKeybind.Key9k2,
        FluXisGameplayKeybind.Key9k3,
        FluXisGameplayKeybind.Key9k4,
        FluXisGameplayKeybind.Key9k5,
        FluXisGameplayKeybind.Key9k6,
        FluXisGameplayKeybind.Key9k7,
        FluXisGameplayKeybind.Key9k8,
        FluXisGameplayKeybind.Key9k9
    };

    private static readonly FluXisGameplayKeybind[] keys10 =
    {
        FluXisGameplayKeybind.Key10k1,
        FluXisGameplayKeybind.Key10k2,
        FluXisGameplayKeybind.Key10k3,
        FluXisGameplayKeybind.Key10k4,
        FluXisGameplayKeybind.Key10k5,
        FluXisGameplayKeybind.Key10k6,
        FluXisGameplayKeybind.Key10k7,
        FluXisGameplayKeybind.Key10k8,
        FluXisGameplayKeybind.Key10k9,
        FluXisGameplayKeybind.Key10k10
    };

    private readonly bool[] pressedStatus;
    public bool[] JustPressed { get; }
    public bool[] Pressed { get; }
    public bool[] JustReleased { get; }
    public FluXisGameplayKeybind[] Keys { get; }

    private readonly GameplayScreen screen;

    public GameplayInput(GameplayScreen screen, int mode = 4)
    {
        this.screen = screen;

        Keys = mode switch
        {
            1 => keys1,
            2 => keys2,
            3 => keys3,
            4 => keys4,
            5 => keys5,
            6 => keys6,
            7 => keys7,
            8 => keys8,
            9 => keys9,
            10 => keys10,
            _ => Array.Empty<FluXisGameplayKeybind>()
        };

        pressedStatus = new bool[mode];
        JustPressed = new bool[mode];
        Pressed = new bool[mode];
        JustReleased = new bool[mode];
    }

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < Keys.Length; i++)
        {
            switch (pressedStatus[i])
            {
                case true when !Pressed[i]:
                    JustPressed[i] = true;
                    Pressed[i] = true;
                    break;

                case false when Pressed[i]:
                    JustReleased[i] = true;
                    Pressed[i] = false;
                    break;

                default:
                    JustPressed[i] = false;
                    JustReleased[i] = false;
                    break;
            }
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => PressKey(e.Action);

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) => ReleaseKey(e.Action);

    public bool PressKey(FluXisGameplayKeybind key)
    {
        if (screen.IsPaused.Value || screen.Playfield.Manager.AutoPlay)
            return false;

        for (int i = 0; i < Keys.Length; i++)
        {
            if (key == Keys[i])
            {
                pressedStatus[i] = true;
                return true;
            }
        }

        return false;
    }

    public void ReleaseKey(FluXisGameplayKeybind key)
    {
        if (screen.IsPaused.Value || screen.Playfield.Manager.AutoPlay)
            return;

        for (int i = 0; i < Keys.Length; i++)
        {
            if (key == Keys[i])
                pressedStatus[i] = false;
        }
    }
}
