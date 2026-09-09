using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using fluXis.Modes.Gameplay.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.States;

namespace fluXis.Modes.Keys.Gameplay;

public partial class KeysKeybindContainer : GameModeKeybindContainer<FluXisGameplayKeybind>
{
    public override IEnumerable<IKeyBinding> DefaultKeyBindings { get; }

    public bool[] Pressed { get; }
    public double[] PressTimes { get; }
    public FluXisGameplayKeybind[] Keys { get; }

    public KeysKeybindContainer(int mode, bool dual)
    {
        DefaultKeyBindings = GetKeys(mode, dual);
        Keys = [.. DefaultKeyBindings.Select(x => (FluXisGameplayKeybind)x.Action)];

        if (dual)
            mode *= 2;

        Pressed = new bool[mode];
        PressTimes = new double[mode];
    }

    protected override Drawable PropagatePressed(IEnumerable<Drawable> drawables, InputState state, FluXisGameplayKeybind pressed, float scrollAmount = 0, bool isPrecise = false, bool repeat = false)
    {
        var idx = Array.IndexOf(Keys, pressed);
        if (idx == -1) return null;

        Pressed[idx] = true;
        PressTimes[idx] = Time.Current;

        return base.PropagatePressed(drawables, state, pressed, scrollAmount, isPrecise, repeat);
    }

    protected override void PropagateReleased(IEnumerable<Drawable> drawables, InputState state, FluXisGameplayKeybind released)
    {
        var idx = Array.IndexOf(Keys, released);
        if (idx == -1) return;

        Pressed[idx] = false;
        PressTimes[idx] = 0;

        base.PropagateReleased(drawables, state, released);
    }

    public static IEnumerable<KeyBinding> GetKeys(int mode, bool dual)
    {
        var binds = new List<FluXisGameplayKeybind>();

        if (mode is >= 1 and <= 10)
        {
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
        }
        else
            binds = Enum.GetValues<FluXisGameplayKeybind>().ToList();

        return binds.Select(b => new KeyBinding(GetDefaultFor(b), b));
    }

    public static InputKey GetDefaultFor(FluXisGameplayKeybind bind) => bind switch
    {
        FluXisGameplayKeybind.Key1k1 => InputKey.Space,

        FluXisGameplayKeybind.Key2k1 => InputKey.D,
        FluXisGameplayKeybind.Key2k2 => InputKey.J,

        FluXisGameplayKeybind.Key3k1 => InputKey.D,
        FluXisGameplayKeybind.Key3k2 => InputKey.Space,
        FluXisGameplayKeybind.Key3k3 => InputKey.J,

        FluXisGameplayKeybind.Key4k1 => InputKey.S,
        FluXisGameplayKeybind.Key4k2 => InputKey.D,
        FluXisGameplayKeybind.Key4k3 => InputKey.J,
        FluXisGameplayKeybind.Key4k4 => InputKey.K,

        FluXisGameplayKeybind.Key5k1 => InputKey.S,
        FluXisGameplayKeybind.Key5k2 => InputKey.D,
        FluXisGameplayKeybind.Key5k3 => InputKey.Space,
        FluXisGameplayKeybind.Key5k4 => InputKey.J,
        FluXisGameplayKeybind.Key5k5 => InputKey.K,

        FluXisGameplayKeybind.Key6k1 => InputKey.A,
        FluXisGameplayKeybind.Key6k2 => InputKey.S,
        FluXisGameplayKeybind.Key6k3 => InputKey.D,
        FluXisGameplayKeybind.Key6k4 => InputKey.J,
        FluXisGameplayKeybind.Key6k5 => InputKey.K,
        FluXisGameplayKeybind.Key6k6 => InputKey.L,

        FluXisGameplayKeybind.Key7k1 => InputKey.A,
        FluXisGameplayKeybind.Key7k2 => InputKey.S,
        FluXisGameplayKeybind.Key7k3 => InputKey.D,
        FluXisGameplayKeybind.Key7k4 => InputKey.Space,
        FluXisGameplayKeybind.Key7k5 => InputKey.J,
        FluXisGameplayKeybind.Key7k6 => InputKey.K,
        FluXisGameplayKeybind.Key7k7 => InputKey.L,

        FluXisGameplayKeybind.Key8k1 => InputKey.A,
        FluXisGameplayKeybind.Key8k2 => InputKey.S,
        FluXisGameplayKeybind.Key8k3 => InputKey.D,
        FluXisGameplayKeybind.Key8k4 => InputKey.V,
        FluXisGameplayKeybind.Key8k5 => InputKey.B,
        FluXisGameplayKeybind.Key8k6 => InputKey.J,
        FluXisGameplayKeybind.Key8k7 => InputKey.K,
        FluXisGameplayKeybind.Key8k8 => InputKey.L,

        FluXisGameplayKeybind.Key9k1 => InputKey.A,
        FluXisGameplayKeybind.Key9k2 => InputKey.S,
        FluXisGameplayKeybind.Key9k3 => InputKey.D,
        FluXisGameplayKeybind.Key9k4 => InputKey.F,
        FluXisGameplayKeybind.Key9k5 => InputKey.Space,
        FluXisGameplayKeybind.Key9k6 => InputKey.H,
        FluXisGameplayKeybind.Key9k7 => InputKey.J,
        FluXisGameplayKeybind.Key9k8 => InputKey.K,
        FluXisGameplayKeybind.Key9k9 => InputKey.L,

        FluXisGameplayKeybind.Key10k1 => InputKey.A,
        FluXisGameplayKeybind.Key10k2 => InputKey.S,
        FluXisGameplayKeybind.Key10k3 => InputKey.D,
        FluXisGameplayKeybind.Key10k4 => InputKey.F,
        FluXisGameplayKeybind.Key10k5 => InputKey.V,
        FluXisGameplayKeybind.Key10k6 => InputKey.B,
        FluXisGameplayKeybind.Key10k7 => InputKey.H,
        FluXisGameplayKeybind.Key10k8 => InputKey.J,
        FluXisGameplayKeybind.Key10k9 => InputKey.K,
        FluXisGameplayKeybind.Key10k10 => InputKey.L,

        FluXisGameplayKeybind.Key1k1D => InputKey.G,

        FluXisGameplayKeybind.Key2k1D => InputKey.E,
        FluXisGameplayKeybind.Key2k2D => InputKey.U,

        FluXisGameplayKeybind.Key3k1D => InputKey.E,
        FluXisGameplayKeybind.Key3k2D => InputKey.G,
        FluXisGameplayKeybind.Key3k3D => InputKey.U,

        FluXisGameplayKeybind.Key4k1D => InputKey.E,
        FluXisGameplayKeybind.Key4k2D => InputKey.R,
        FluXisGameplayKeybind.Key4k3D => InputKey.U,
        FluXisGameplayKeybind.Key4k4D => InputKey.I,

        FluXisGameplayKeybind.Key5k1D => InputKey.E,
        FluXisGameplayKeybind.Key5k2D => InputKey.R,
        FluXisGameplayKeybind.Key5k3D => InputKey.G,
        FluXisGameplayKeybind.Key5k4D => InputKey.U,
        FluXisGameplayKeybind.Key5k5D => InputKey.I,

        FluXisGameplayKeybind.Key6k1D => InputKey.W,
        FluXisGameplayKeybind.Key6k2D => InputKey.E,
        FluXisGameplayKeybind.Key6k3D => InputKey.R,
        FluXisGameplayKeybind.Key6k4D => InputKey.U,
        FluXisGameplayKeybind.Key6k5D => InputKey.I,
        FluXisGameplayKeybind.Key6k6D => InputKey.O,

        FluXisGameplayKeybind.Key7k1D => InputKey.W,
        FluXisGameplayKeybind.Key7k2D => InputKey.E,
        FluXisGameplayKeybind.Key7k3D => InputKey.R,
        FluXisGameplayKeybind.Key7k4D => InputKey.G,
        FluXisGameplayKeybind.Key7k5D => InputKey.U,
        FluXisGameplayKeybind.Key7k6D => InputKey.I,
        FluXisGameplayKeybind.Key7k7D => InputKey.O,

        FluXisGameplayKeybind.Key8k1D => InputKey.W,
        FluXisGameplayKeybind.Key8k2D => InputKey.E,
        FluXisGameplayKeybind.Key8k3D => InputKey.R,
        FluXisGameplayKeybind.Key8k4D => InputKey.G,
        FluXisGameplayKeybind.Key8k5D => InputKey.H,
        FluXisGameplayKeybind.Key8k6D => InputKey.U,
        FluXisGameplayKeybind.Key8k7D => InputKey.I,
        FluXisGameplayKeybind.Key8k8D => InputKey.O,

        FluXisGameplayKeybind.Key9k1D => InputKey.Q,
        FluXisGameplayKeybind.Key9k2D => InputKey.W,
        FluXisGameplayKeybind.Key9k3D => InputKey.E,
        FluXisGameplayKeybind.Key9k4D => InputKey.R,
        FluXisGameplayKeybind.Key9k5D => InputKey.G,
        FluXisGameplayKeybind.Key9k6D => InputKey.U,
        FluXisGameplayKeybind.Key9k7D => InputKey.I,
        FluXisGameplayKeybind.Key9k8D => InputKey.O,
        FluXisGameplayKeybind.Key9k9D => InputKey.P,

        FluXisGameplayKeybind.Key10k1D => InputKey.Q,
        FluXisGameplayKeybind.Key10k2D => InputKey.W,
        FluXisGameplayKeybind.Key10k3D => InputKey.E,
        FluXisGameplayKeybind.Key10k4D => InputKey.R,
        FluXisGameplayKeybind.Key10k5D => InputKey.G,
        FluXisGameplayKeybind.Key10k6D => InputKey.H,
        FluXisGameplayKeybind.Key10k7D => InputKey.U,
        FluXisGameplayKeybind.Key10k8D => InputKey.I,
        FluXisGameplayKeybind.Key10k9D => InputKey.O,
        FluXisGameplayKeybind.Key10k10D => InputKey.P,

        _ => throw new ArgumentOutOfRangeException(nameof(bind), bind, null)
    };
}
