using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Input;

public partial class GameplayInput : Drawable, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public bool[] Pressed { get; }
    public double[] PressTimes { get; }
    public List<FluXisGameplayKeybind> Keys { get; }

    public event Action<FluXisGameplayKeybind> OnPress;
    public event Action<FluXisGameplayKeybind> OnRelease;

    public readonly bool Dual;
    private readonly Bindable<bool> paused;

    public GameplayInput(Bindable<bool> paused, int mode = 4, bool dual = false)
    {
        this.paused = paused;
        Dual = dual;

        Keys = GetKeys(mode, dual).ToList();

        if (dual)
            mode *= 2;

        Pressed = new bool[mode];
        PressTimes = new double[mode];
    }

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => !e.Repeat && PressKey(e.Action);
    public virtual void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) => ReleaseKey(e.Action);

    public bool PressKey(FluXisGameplayKeybind key)
    {
        if (paused.Value)
            return false;

        var idx = Keys.IndexOf(key);
        if (idx == -1) return false;

        Pressed[idx] = true;
        PressTimes[idx] = Time.Current;
        OnPress?.Invoke(key);
        return true;
    }

    public void ReleaseKey(FluXisGameplayKeybind key)
    {
        if (paused.Value)
            return;

        var idx = Keys.IndexOf(key);
        if (idx == -1) return;

        Pressed[idx] = false;
        PressTimes[idx] = 0;
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
