using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Input.Bindings;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input;

public partial class GameplayKeybindContainer : RealmKeyBindingContainer<FluXisGameplayKeybind>
{
    public override IEnumerable<IKeyBinding> DefaultKeyBindings { get; }

    public GameplayKeybindContainer(FluXisRealm realm, int keyCount, bool dual = false)
        : base(realm, SimultaneousBindingMode.Unique)
    {
        DefaultKeyBindings = getBindings(keyCount, dual);
    }

    private IEnumerable<KeyBinding> getBindings(int mode, bool dual)
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

        return binds.Select(b => new KeyBinding(getDefaultFor(b), b));
    }

    private InputKey getDefaultFor(FluXisGameplayKeybind bind) => bind switch
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
        FluXisGameplayKeybind.Key8k4 => InputKey.F,
        FluXisGameplayKeybind.Key8k5 => InputKey.H,
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

        FluXisGameplayKeybind.Key1k1D => InputKey.Space,

        FluXisGameplayKeybind.Key2k1D => InputKey.D,
        FluXisGameplayKeybind.Key2k2D => InputKey.J,

        FluXisGameplayKeybind.Key3k1D => InputKey.D,
        FluXisGameplayKeybind.Key3k2D => InputKey.Space,
        FluXisGameplayKeybind.Key3k3D => InputKey.J,

        FluXisGameplayKeybind.Key4k1D => InputKey.S,
        FluXisGameplayKeybind.Key4k2D => InputKey.D,
        FluXisGameplayKeybind.Key4k3D => InputKey.J,
        FluXisGameplayKeybind.Key4k4D => InputKey.K,

        FluXisGameplayKeybind.Key5k1D => InputKey.S,
        FluXisGameplayKeybind.Key5k2D => InputKey.D,
        FluXisGameplayKeybind.Key5k3D => InputKey.Space,
        FluXisGameplayKeybind.Key5k4D => InputKey.J,
        FluXisGameplayKeybind.Key5k5D => InputKey.K,

        FluXisGameplayKeybind.Key6k1D => InputKey.A,
        FluXisGameplayKeybind.Key6k2D => InputKey.S,
        FluXisGameplayKeybind.Key6k3D => InputKey.D,
        FluXisGameplayKeybind.Key6k4D => InputKey.J,
        FluXisGameplayKeybind.Key6k5D => InputKey.K,
        FluXisGameplayKeybind.Key6k6D => InputKey.L,

        FluXisGameplayKeybind.Key7k1D => InputKey.A,
        FluXisGameplayKeybind.Key7k2D => InputKey.S,
        FluXisGameplayKeybind.Key7k3D => InputKey.D,
        FluXisGameplayKeybind.Key7k4D => InputKey.Space,
        FluXisGameplayKeybind.Key7k5D => InputKey.J,
        FluXisGameplayKeybind.Key7k6D => InputKey.K,
        FluXisGameplayKeybind.Key7k7D => InputKey.L,

        FluXisGameplayKeybind.Key8k1D => InputKey.A,
        FluXisGameplayKeybind.Key8k2D => InputKey.S,
        FluXisGameplayKeybind.Key8k3D => InputKey.D,
        FluXisGameplayKeybind.Key8k4D => InputKey.F,
        FluXisGameplayKeybind.Key8k5D => InputKey.H,
        FluXisGameplayKeybind.Key8k6D => InputKey.J,
        FluXisGameplayKeybind.Key8k7D => InputKey.K,
        FluXisGameplayKeybind.Key8k8D => InputKey.L,

        FluXisGameplayKeybind.Key9k1D => InputKey.A,
        FluXisGameplayKeybind.Key9k2D => InputKey.S,
        FluXisGameplayKeybind.Key9k3D => InputKey.D,
        FluXisGameplayKeybind.Key9k4D => InputKey.F,
        FluXisGameplayKeybind.Key9k5D => InputKey.Space,
        FluXisGameplayKeybind.Key9k6D => InputKey.H,
        FluXisGameplayKeybind.Key9k7D => InputKey.J,
        FluXisGameplayKeybind.Key9k8D => InputKey.K,
        FluXisGameplayKeybind.Key9k9D => InputKey.L,

        FluXisGameplayKeybind.Key10k1D => InputKey.A,
        FluXisGameplayKeybind.Key10k2D => InputKey.S,
        FluXisGameplayKeybind.Key10k3D => InputKey.D,
        FluXisGameplayKeybind.Key10k4D => InputKey.F,
        FluXisGameplayKeybind.Key10k5D => InputKey.V,
        FluXisGameplayKeybind.Key10k6D => InputKey.B,
        FluXisGameplayKeybind.Key10k7D => InputKey.H,
        FluXisGameplayKeybind.Key10k8D => InputKey.J,
        FluXisGameplayKeybind.Key10k9D => InputKey.K,
        FluXisGameplayKeybind.Key10k10D => InputKey.L,

        _ => throw new ArgumentOutOfRangeException(nameof(bind), bind, null)
    };
}

// ReSharper disable InconsistentNaming
public enum FluXisGameplayKeybind
{
    // 1k
    Key1k1,

    // 2k
    Key2k1,
    Key2k2,

    // 3k
    Key3k1,
    Key3k2,
    Key3k3,

    // 4k
    Key4k1,
    Key4k2,
    Key4k3,
    Key4k4,

    // 5k
    Key5k1,
    Key5k2,
    Key5k3,
    Key5k4,
    Key5k5,

    // 6k
    Key6k1,
    Key6k2,
    Key6k3,
    Key6k4,
    Key6k5,
    Key6k6,

    // 7k
    Key7k1,
    Key7k2,
    Key7k3,
    Key7k4,
    Key7k5,
    Key7k6,
    Key7k7,

    // 8k
    Key8k1,
    Key8k2,
    Key8k3,
    Key8k4,
    Key8k5,
    Key8k6,
    Key8k7,
    Key8k8,

    // 9k
    Key9k1,
    Key9k2,
    Key9k3,
    Key9k4,
    Key9k5,
    Key9k6,
    Key9k7,
    Key9k8,
    Key9k9,

    // 10k
    Key10k1,
    Key10k2,
    Key10k3,
    Key10k4,
    Key10k5,
    Key10k6,
    Key10k7,
    Key10k8,
    Key10k9,
    Key10k10,

    // 1k dual
    Key1k1D,

    // 2k dual
    Key2k1D,
    Key2k2D,

    // 3k dual
    Key3k1D,
    Key3k2D,
    Key3k3D,

    // 4k dual
    Key4k1D,
    Key4k2D,
    Key4k3D,
    Key4k4D,

    // 5k dual
    Key5k1D,
    Key5k2D,
    Key5k3D,
    Key5k4D,
    Key5k5D,

    // 6k dual
    Key6k1D,
    Key6k2D,
    Key6k3D,
    Key6k4D,
    Key6k5D,
    Key6k6D,

    // 7k dual
    Key7k1D,
    Key7k2D,
    Key7k3D,
    Key7k4D,
    Key7k5D,
    Key7k6D,
    Key7k7D,

    // 8k dual
    Key8k1D,
    Key8k2D,
    Key8k3D,
    Key8k4D,
    Key8k5D,
    Key8k6D,
    Key8k7D,
    Key8k8D,

    // 9k dual
    Key9k1D,
    Key9k2D,
    Key9k3D,
    Key9k4D,
    Key9k5D,
    Key9k6D,
    Key9k7D,
    Key9k8D,
    Key9k9D,

    // 10k dual
    Key10k1D,
    Key10k2D,
    Key10k3D,
    Key10k4D,
    Key10k5D,
    Key10k6D,
    Key10k7D,
    Key10k8D,
    Key10k9D,
    Key10k10D,
}
