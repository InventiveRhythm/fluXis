using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input;

public partial class GameplayKeybindContainer : RealmKeyBindingContainer<FluXisGameplayKeybind>
{
    private readonly FluXisGameBase handler;

    private InputManager inputManager;

    public GameplayKeybindContainer(FluXisGameBase game, FluXisRealm realm)
        : base(realm, SimultaneousBindingMode.All, KeyCombinationMatchingMode.Any)
    {
        this.handler = game;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
    }

    public override IEnumerable<IKeyBinding> DefaultKeyBindings => KeyBindings1K
                                                                   .Concat(KeyBindings2K)
                                                                   .Concat(KeyBindings3K)
                                                                   .Concat(KeyBindings4K)
                                                                   .Concat(KeyBindings5K)
                                                                   .Concat(KeyBindings6K)
                                                                   .Concat(KeyBindings7K)
                                                                   .Concat(KeyBindings8K)
                                                                   .Concat(KeyBindings9K)
                                                                   .Concat(KeyBindings10K);

    protected override IEnumerable<Drawable> KeyBindingInputQueue
    {
        get
        {
            var queue = inputManager?.NonPositionalInputQueue ?? base.KeyBindingInputQueue;

            if (handler != null)
                queue = queue.Prepend(handler);

            return queue;
        }
    }

    public IEnumerable<KeyBinding> KeyBindings1K = new[]
    {
        new KeyBinding(InputKey.Space, FluXisGameplayKeybind.Key1k1),
    };

    public IEnumerable<KeyBinding> KeyBindings2K = new[]
    {
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key2k1),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key2k2)
    };

    public IEnumerable<KeyBinding> KeyBindings3K = new[]
    {
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key3k1),
        new KeyBinding(InputKey.Space, FluXisGameplayKeybind.Key3k2),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key3k3)
    };

    public IEnumerable<KeyBinding> KeyBindings4K = new[]
    {
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key4k1),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key4k2),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key4k3),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key4k4)
    };

    public IEnumerable<KeyBinding> KeyBindings5K = new[]
    {
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key5k1),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key5k2),
        new KeyBinding(InputKey.Space, FluXisGameplayKeybind.Key5k3),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key5k4),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key5k5)
    };

    public IEnumerable<KeyBinding> KeyBindings6K = new[]
    {
        new KeyBinding(InputKey.A, FluXisGameplayKeybind.Key6k1),
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key6k2),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key6k3),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key6k4),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key6k5),
        new KeyBinding(InputKey.L, FluXisGameplayKeybind.Key6k6)
    };

    public IEnumerable<KeyBinding> KeyBindings7K = new[]
    {
        new KeyBinding(InputKey.A, FluXisGameplayKeybind.Key7k1),
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key7k2),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key7k3),
        new KeyBinding(InputKey.Space, FluXisGameplayKeybind.Key7k4),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key7k5),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key7k6),
        new KeyBinding(InputKey.L, FluXisGameplayKeybind.Key7k7)
    };

    public IEnumerable<KeyBinding> KeyBindings8K = new[]
    {
        new KeyBinding(InputKey.A, FluXisGameplayKeybind.Key8k1),
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key8k2),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key8k3),
        new KeyBinding(InputKey.F, FluXisGameplayKeybind.Key8k4),
        new KeyBinding(InputKey.H, FluXisGameplayKeybind.Key8k5),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key8k6),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key8k7),
        new KeyBinding(InputKey.L, FluXisGameplayKeybind.Key8k8)
    };

    public IEnumerable<KeyBinding> KeyBindings9K = new[]
    {
        new KeyBinding(InputKey.A, FluXisGameplayKeybind.Key9k1),
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key9k2),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key9k3),
        new KeyBinding(InputKey.F, FluXisGameplayKeybind.Key9k4),
        new KeyBinding(InputKey.Space, FluXisGameplayKeybind.Key9k5),
        new KeyBinding(InputKey.H, FluXisGameplayKeybind.Key9k6),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key9k7),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key9k8),
        new KeyBinding(InputKey.L, FluXisGameplayKeybind.Key9k9)
    };

    public IEnumerable<KeyBinding> KeyBindings10K = new[]
    {
        new KeyBinding(InputKey.A, FluXisGameplayKeybind.Key10k1),
        new KeyBinding(InputKey.S, FluXisGameplayKeybind.Key10k2),
        new KeyBinding(InputKey.D, FluXisGameplayKeybind.Key10k3),
        new KeyBinding(InputKey.F, FluXisGameplayKeybind.Key10k4),
        new KeyBinding(InputKey.V, FluXisGameplayKeybind.Key10k5),
        new KeyBinding(InputKey.B, FluXisGameplayKeybind.Key10k6),
        new KeyBinding(InputKey.H, FluXisGameplayKeybind.Key10k7),
        new KeyBinding(InputKey.J, FluXisGameplayKeybind.Key10k8),
        new KeyBinding(InputKey.K, FluXisGameplayKeybind.Key10k9),
        new KeyBinding(InputKey.L, FluXisGameplayKeybind.Key10k10)
    };
}

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
}
