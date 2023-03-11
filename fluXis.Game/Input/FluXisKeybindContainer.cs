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
    private FluXisRealm realm;

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
        new KeyBinding(InputKey.Enter, FluXisKeybind.Select),
        new KeyBinding(InputKey.Space, FluXisKeybind.Select),
        new KeyBinding(InputKey.Escape, FluXisKeybind.Back),
        new KeyBinding(InputKey.ExtraMouseButton4, FluXisKeybind.Back),
        new KeyBinding(InputKey.Up, FluXisKeybind.Previous),
        new KeyBinding(InputKey.Down, FluXisKeybind.Next),
        new KeyBinding(InputKey.Left, FluXisKeybind.PreviousGroup),
        new KeyBinding(InputKey.Right, FluXisKeybind.NextGroup),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.O), FluXisKeybind.ToggleSettings),
        new KeyBinding(InputKey.Delete, FluXisKeybind.Delete),

        getBinding(FluXisKeybind.Key4k1, InputKey.A),
        getBinding(FluXisKeybind.Key4k2, InputKey.S),
        getBinding(FluXisKeybind.Key4k3, InputKey.K),
        getBinding(FluXisKeybind.Key4k4, InputKey.L),

        getBinding(FluXisKeybind.Key5k1, InputKey.A),
        getBinding(FluXisKeybind.Key5k2, InputKey.S),
        getBinding(FluXisKeybind.Key5k3, InputKey.Space),
        getBinding(FluXisKeybind.Key5k4, InputKey.K),
        getBinding(FluXisKeybind.Key5k5, InputKey.L),

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

        new KeyBinding(InputKey.Space, FluXisKeybind.Skip),
        new KeyBinding(InputKey.Shift, FluXisKeybind.Restart),
        new KeyBinding(InputKey.Control, FluXisKeybind.Exit),
        new KeyBinding(InputKey.Escape, FluXisKeybind.Pause),
        new KeyBinding(InputKey.Left, FluXisKeybind.SeekBackward),
        new KeyBinding(InputKey.Right, FluXisKeybind.SeekForward),
        new KeyBinding(InputKey.F3, FluXisKeybind.ScrollSpeedDecrease),
        new KeyBinding(InputKey.F4, FluXisKeybind.ScrollSpeedIncrease),

        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.A), FluXisKeybind.ToggleAutoplay)
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

    private KeyBinding getBinding(FluXisKeybind action, InputKey key)
    {
        KeyBinding bind = null;

        realm.Run(r =>
        {
            var binding = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == action.ToString());

            if (binding == null)
            {
                binding = new RealmKeybind
                {
                    Key = key.ToString(),
                    Action = action.ToString(),
                };
                r.Write(() =>
                {
                    r.Add(binding);
                });
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
    Delete,

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

    // Ingame
    Skip,
    Restart,
    Exit,
    Pause,
    SeekBackward,
    SeekForward,
    ScrollSpeedIncrease,
    ScrollSpeedDecrease,

    // Ingame (Debug)
    ToggleAutoplay
}
