using System;
using fluXis.Game.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Input;

public partial class GameplayInput : Drawable, IKeyBindingHandler<FluXisKeybind>
{
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

    private readonly FluXisKeybind[] keys;

    private readonly bool[] pressedStatus;
    public bool[] JustPressed { get; }
    public bool[] Pressed { get; }
    public bool[] JustReleased { get; }

    private readonly GameplayScreen screen;

    public GameplayInput(GameplayScreen screen, int mode = 4)
    {
        this.screen = screen;

        int keyCount = mode switch
        {
            4 => 4,
            5 => 5,
            6 => 6,
            7 => 7,
            _ => 4
        };

        keys = keyCount switch
        {
            4 => keys4,
            5 => keys5,
            6 => keys6,
            7 => keys7,
            _ => throw new ArgumentOutOfRangeException() // Should never happen lmao
        };

        pressedStatus = new bool[keyCount];
        JustPressed = new bool[keyCount];
        Pressed = new bool[keyCount];
        JustReleased = new bool[keyCount];
    }

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < keys.Length; i++)
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

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        if (screen.IsPaused.Value || screen.Playfield.Manager.AutoPlay)
            return false;

        for (int i = 0; i < keys.Length; i++)
        {
            if (e.Action == keys[i])
            {
                pressedStatus[i] = true;
                return true;
            }
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
    {
        if (screen.IsPaused.Value || screen.Playfield.Manager.AutoPlay)
            return;

        for (int i = 0; i < keys.Length; i++)
        {
            if (e.Action == keys[i])
            {
                pressedStatus[i] = false;
            }
        }
    }
}
