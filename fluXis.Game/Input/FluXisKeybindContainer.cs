using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input;

public partial class FluXisKeybindContainer : KeyBindingContainer<FluXisKeybind>
{
    private readonly Drawable handleInput;
    private InputManager inputManager;
    private readonly FluXisRealm realm;

    public FluXisKeybindContainer(Drawable game, FluXisRealm realm)
        : base(matchingMode: KeyCombinationMatchingMode.Modifiers, simultaneousMode: SimultaneousBindingMode.All)
    {
        if (game != null)
            handleInput = game;

        this.realm = realm;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
    }

    public override IEnumerable<IKeyBinding> DefaultKeyBindings => new[]
    {
        getBinding(FluXisKeybind.Select, InputKey.Enter),
        getBinding(FluXisKeybind.Back, InputKey.Escape),
        new KeyBinding(InputKey.ExtraMouseButton1, FluXisKeybind.Back),
        getBinding(FluXisKeybind.Previous, InputKey.Up),
        getBinding(FluXisKeybind.Next, InputKey.Down),
        getBinding(FluXisKeybind.PreviousGroup, InputKey.Left),
        getBinding(FluXisKeybind.NextGroup, InputKey.Right),
        getBinding(FluXisKeybind.ToggleSettings, new KeyCombination(InputKey.Control, InputKey.O)),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Shift, InputKey.S), FluXisKeybind.OpenSkinEditor),
        getBinding(FluXisKeybind.ToggleDashboard, new KeyCombination(InputKey.Control, InputKey.D)),
        getBinding(FluXisKeybind.Home, new KeyCombination(InputKey.Control, InputKey.H)),
        getBinding(FluXisKeybind.Delete, InputKey.Delete),
        getBinding(FluXisKeybind.MusicPrevious, InputKey.F5),
        getBinding(FluXisKeybind.MusicPause, InputKey.F6),
        getBinding(FluXisKeybind.MusicNext, InputKey.F7),
        getBinding(FluXisKeybind.ToggleMusicPlayer, InputKey.F8),

        getBinding(FluXisKeybind.VolumeDecrease, new KeyCombination(InputKey.Alt, InputKey.Left)),
        getBinding(FluXisKeybind.VolumeIncrease, new KeyCombination(InputKey.Alt, InputKey.Right)),
        getBinding(FluXisKeybind.VolumePreviousCategory, new KeyCombination(InputKey.Alt, InputKey.Up)),
        getBinding(FluXisKeybind.VolumeNextCategory, new KeyCombination(InputKey.Alt, InputKey.Down)),

        getBinding(FluXisKeybind.Key1k1, InputKey.Space),

        getBinding(FluXisKeybind.Key2k1, InputKey.D),
        getBinding(FluXisKeybind.Key2k2, InputKey.J),

        getBinding(FluXisKeybind.Key3k1, InputKey.D),
        getBinding(FluXisKeybind.Key3k2, InputKey.Space),
        getBinding(FluXisKeybind.Key3k3, InputKey.J),

        getBinding(FluXisKeybind.Key4k1, InputKey.S),
        getBinding(FluXisKeybind.Key4k2, InputKey.D),
        getBinding(FluXisKeybind.Key4k3, InputKey.J),
        getBinding(FluXisKeybind.Key4k4, InputKey.K),

        getBinding(FluXisKeybind.Key5k1, InputKey.S),
        getBinding(FluXisKeybind.Key5k2, InputKey.D),
        getBinding(FluXisKeybind.Key5k3, InputKey.Space),
        getBinding(FluXisKeybind.Key5k4, InputKey.J),
        getBinding(FluXisKeybind.Key5k5, InputKey.K),

        getBinding(FluXisKeybind.Key6k1, InputKey.A),
        getBinding(FluXisKeybind.Key6k2, InputKey.S),
        getBinding(FluXisKeybind.Key6k3, InputKey.D),
        getBinding(FluXisKeybind.Key6k4, InputKey.J),
        getBinding(FluXisKeybind.Key6k5, InputKey.K),
        getBinding(FluXisKeybind.Key6k6, InputKey.L),

        getBinding(FluXisKeybind.Key7k1, InputKey.A),
        getBinding(FluXisKeybind.Key7k2, InputKey.S),
        getBinding(FluXisKeybind.Key7k3, InputKey.D),
        getBinding(FluXisKeybind.Key7k4, InputKey.Space),
        getBinding(FluXisKeybind.Key7k5, InputKey.J),
        getBinding(FluXisKeybind.Key7k6, InputKey.K),
        getBinding(FluXisKeybind.Key7k7, InputKey.L),

        getBinding(FluXisKeybind.Key8k1, InputKey.A),
        getBinding(FluXisKeybind.Key8k2, InputKey.S),
        getBinding(FluXisKeybind.Key8k3, InputKey.D),
        getBinding(FluXisKeybind.Key8k4, InputKey.F),
        getBinding(FluXisKeybind.Key8k5, InputKey.H),
        getBinding(FluXisKeybind.Key8k6, InputKey.J),
        getBinding(FluXisKeybind.Key8k7, InputKey.K),
        getBinding(FluXisKeybind.Key8k8, InputKey.L),

        getBinding(FluXisKeybind.Key9k1, InputKey.A),
        getBinding(FluXisKeybind.Key9k2, InputKey.S),
        getBinding(FluXisKeybind.Key9k3, InputKey.D),
        getBinding(FluXisKeybind.Key9k4, InputKey.F),
        getBinding(FluXisKeybind.Key9k5, InputKey.Space),
        getBinding(FluXisKeybind.Key9k6, InputKey.H),
        getBinding(FluXisKeybind.Key9k7, InputKey.J),
        getBinding(FluXisKeybind.Key9k8, InputKey.K),
        getBinding(FluXisKeybind.Key9k9, InputKey.L),

        getBinding(FluXisKeybind.Key10k1, InputKey.A),
        getBinding(FluXisKeybind.Key10k2, InputKey.S),
        getBinding(FluXisKeybind.Key10k3, InputKey.D),
        getBinding(FluXisKeybind.Key10k4, InputKey.F),
        getBinding(FluXisKeybind.Key10k5, InputKey.V),
        getBinding(FluXisKeybind.Key10k6, InputKey.B),
        getBinding(FluXisKeybind.Key10k7, InputKey.H),
        getBinding(FluXisKeybind.Key10k8, InputKey.J),
        getBinding(FluXisKeybind.Key10k9, InputKey.K),
        getBinding(FluXisKeybind.Key10k10, InputKey.L),

        getBinding(FluXisKeybind.Skip, InputKey.Space),
        getBinding(FluXisKeybind.QuickRestart, InputKey.Shift),
        getBinding(FluXisKeybind.QuickExit, InputKey.Control),
        new KeyBinding(InputKey.Escape, FluXisKeybind.GameplayPause),
        getBinding(FluXisKeybind.SeekBackward, InputKey.Left),
        getBinding(FluXisKeybind.SeekForward, InputKey.Right),
        getBinding(FluXisKeybind.ScrollSpeedDecrease, InputKey.F3),
        getBinding(FluXisKeybind.ScrollSpeedIncrease, InputKey.F4),
    };

    protected override IEnumerable<Drawable> KeyBindingInputQueue
    {
        get
        {
            var queue = inputManager?.NonPositionalInputQueue ?? base.KeyBindingInputQueue;

            if (handleInput != null)
                queue = queue.Prepend(handleInput);

            return queue;
        }
    }

    private KeyBinding getBinding(FluXisKeybind action, KeyCombination combo)
    {
        KeyBinding bind = null;

        realm.Run(r =>
        {
            var binding = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == action.ToString());

            if (binding == null)
            {
                r.Write(() => r.Add(binding = new RealmKeybind
                {
                    Key = string.Join(',', combo.Keys.Select(x => x.ToString())),
                    Action = action.ToString()
                }));
            }

            var split = binding.Key.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length > 1)
            {
                bind = new KeyBinding(split.Select(x => (InputKey)Enum.Parse(typeof(InputKey), x)).ToArray(), action);
                return;
            }

            bind = new KeyBinding((InputKey)Enum.Parse(typeof(InputKey), binding.Key), action);
        });

        return bind;
    }

    public void Reload() => ReloadMappings();
}

public enum FluXisKeybind
{
    // Global
    Select,
    Back,
    Previous,
    Next,
    PreviousGroup,
    NextGroup,
    ToggleSettings,
    OpenSkinEditor,
    ToggleDashboard,
    Home,
    Delete,
    MusicPrevious,
    MusicPause,
    MusicNext,
    ToggleMusicPlayer,

    VolumeDecrease,
    VolumeIncrease,
    VolumePreviousCategory,
    VolumeNextCategory,

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

    // Ingame
    Skip,
    QuickRestart,
    QuickExit,
    GameplayPause,
    SeekBackward,
    SeekForward,
    ScrollSpeedIncrease,
    ScrollSpeedDecrease
}
