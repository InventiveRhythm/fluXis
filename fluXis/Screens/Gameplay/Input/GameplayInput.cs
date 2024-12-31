using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Input;

public partial class GameplayInput : Drawable, IKeyBindingHandler<FluXisGameplayKeybind>
{
    private readonly bool[] pressedStatus;
    public bool[] JustPressed { get; }
    public bool[] Pressed { get; }
    public bool[] JustReleased { get; }
    public List<FluXisGameplayKeybind> Keys { get; }

    public event Action<FluXisGameplayKeybind> OnPress;
    public event Action<FluXisGameplayKeybind> OnRelease;

    public readonly bool Dual;
    private readonly GameplayScreen screen;

    public GameplayInput(GameplayScreen screen, int mode = 4, bool dual = false)
    {
        Dual = dual;
        this.screen = screen;

        Keys = GetKeys(mode, dual).ToList();

        if (dual)
            mode *= 2;

        pressedStatus = new bool[mode];
        JustPressed = new bool[mode];
        Pressed = new bool[mode];
        JustReleased = new bool[mode];
    }

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < Keys.Count; i++)
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

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => !e.Repeat && PressKey(e.Action);
    public virtual void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) => ReleaseKey(e.Action);

    public bool PressKey(FluXisGameplayKeybind key)
    {
        if (screen.IsPaused.Value)
            return false;

        var idx = Keys.IndexOf(key);
        if (idx == -1) return false;

        pressedStatus[idx] = true;
        OnPress?.Invoke(key);
        return true;
    }

    public void ReleaseKey(FluXisGameplayKeybind key)
    {
        if (screen.IsPaused.Value)
            return;

        var idx = Keys.IndexOf(key);
        if (idx == -1) return;

        pressedStatus[idx] = false;
        OnRelease?.Invoke(key);
    }

    public static FluXisGameplayKeybind[] GetKeys(int mode, bool dual)
    {
        var binds = new List<FluXisGameplayKeybind>();

        if (mode is < 1 or > 10)
            return Array.Empty<FluXisGameplayKeybind>();

        for (int i = 0; i < mode; i++)
        {
            var str = $"Key{mode}k{i + 1}";
            binds.Add(Enum.Parse<FluXisGameplayKeybind>(str));
        }

        if (dual)
        {
            for (int i = 0; i < mode; i++)
            {
                var str = $"Key{mode}k{i + 1}D";
                binds.Add(Enum.Parse<FluXisGameplayKeybind>(str));
            }
        }

        return binds.ToArray();
    }
}
