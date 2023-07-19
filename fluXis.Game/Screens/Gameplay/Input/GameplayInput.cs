using System;
using fluXis.Game.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Input;

public partial class GameplayInput : Drawable, IKeyBindingHandler<FluXisKeybind>
{
    private static readonly FluXisKeybind[] keys1 =
    {
        FluXisKeybind.Key1k1
    };

    private static readonly FluXisKeybind[] keys2 =
    {
        FluXisKeybind.Key2k1,
        FluXisKeybind.Key2k2
    };

    private static readonly FluXisKeybind[] keys3 =
    {
        FluXisKeybind.Key3k1,
        FluXisKeybind.Key3k2,
        FluXisKeybind.Key3k3
    };

    private static readonly FluXisKeybind[] keys4 =
    {
        FluXisKeybind.Key4k1,
        FluXisKeybind.Key4k2,
        FluXisKeybind.Key4k3,
        FluXisKeybind.Key4k4
    };

    private static readonly FluXisKeybind[] keys5 =
    {
        FluXisKeybind.Key5k1,
        FluXisKeybind.Key5k2,
        FluXisKeybind.Key5k3,
        FluXisKeybind.Key5k4,
        FluXisKeybind.Key5k5
    };

    private static readonly FluXisKeybind[] keys6 =
    {
        FluXisKeybind.Key6k1,
        FluXisKeybind.Key6k2,
        FluXisKeybind.Key6k3,
        FluXisKeybind.Key6k4,
        FluXisKeybind.Key6k5,
        FluXisKeybind.Key6k6
    };

    private static readonly FluXisKeybind[] keys7 =
    {
        FluXisKeybind.Key7k1,
        FluXisKeybind.Key7k2,
        FluXisKeybind.Key7k3,
        FluXisKeybind.Key7k4,
        FluXisKeybind.Key7k5,
        FluXisKeybind.Key7k6,
        FluXisKeybind.Key7k7
    };

    private static readonly FluXisKeybind[] keys8 =
    {
        FluXisKeybind.Key8k1,
        FluXisKeybind.Key8k2,
        FluXisKeybind.Key8k3,
        FluXisKeybind.Key8k4,
        FluXisKeybind.Key8k5,
        FluXisKeybind.Key8k6,
        FluXisKeybind.Key8k7,
        FluXisKeybind.Key8k8
    };

    private static readonly FluXisKeybind[] keys9 =
    {
        FluXisKeybind.Key9k1,
        FluXisKeybind.Key9k2,
        FluXisKeybind.Key9k3,
        FluXisKeybind.Key9k4,
        FluXisKeybind.Key9k5,
        FluXisKeybind.Key9k6,
        FluXisKeybind.Key9k7,
        FluXisKeybind.Key9k8,
        FluXisKeybind.Key9k9
    };

    private static readonly FluXisKeybind[] keys10 =
    {
        FluXisKeybind.Key10k1,
        FluXisKeybind.Key10k2,
        FluXisKeybind.Key10k3,
        FluXisKeybind.Key10k4,
        FluXisKeybind.Key10k5,
        FluXisKeybind.Key10k6,
        FluXisKeybind.Key10k7,
        FluXisKeybind.Key10k8,
        FluXisKeybind.Key10k9,
        FluXisKeybind.Key10k10
    };

    private readonly bool[] pressedStatus;
    public bool[] JustPressed { get; }
    public bool[] Pressed { get; }
    public bool[] JustReleased { get; }
    public FluXisKeybind[] Keys { get; }

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
            _ => Array.Empty<FluXisKeybind>()
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

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e) => PressKey(e.Action);

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) => ReleaseKey(e.Action);

    public bool PressKey(FluXisKeybind key)
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

    public void ReleaseKey(FluXisKeybind key)
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
