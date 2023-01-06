using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input
{
    public class FluXisKeybindContainer : KeyBindingContainer<FluXisKeybind>
    {
        private readonly Drawable handleInput;
        private InputManager inputManager;

        public FluXisKeybindContainer(FluXisGameBase game)
            : base(matchingMode: KeyCombinationMatchingMode.Any, simultaneousMode: SimultaneousBindingMode.All)
        {
            if (game != null)
                handleInput = game;
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
            new KeyBinding(InputKey.Tab, FluXisKeybind.ToggleOnlineOverlay),

            new KeyBinding(InputKey.A, FluXisKeybind.Key4k1),
            new KeyBinding(InputKey.S, FluXisKeybind.Key4k2),
            new KeyBinding(InputKey.K, FluXisKeybind.Key4k3),
            new KeyBinding(InputKey.L, FluXisKeybind.Key4k4),

            new KeyBinding(InputKey.A, FluXisKeybind.Key5k1),
            new KeyBinding(InputKey.S, FluXisKeybind.Key5k2),
            new KeyBinding(InputKey.Space, FluXisKeybind.Key5k3),
            new KeyBinding(InputKey.K, FluXisKeybind.Key5k4),
            new KeyBinding(InputKey.L, FluXisKeybind.Key5k5),

            new KeyBinding(InputKey.A, FluXisKeybind.Key6k1),
            new KeyBinding(InputKey.S, FluXisKeybind.Key6k2),
            new KeyBinding(InputKey.D, FluXisKeybind.Key6k3),
            new KeyBinding(InputKey.J, FluXisKeybind.Key6k4),
            new KeyBinding(InputKey.K, FluXisKeybind.Key6k5),
            new KeyBinding(InputKey.L, FluXisKeybind.Key6k6),

            new KeyBinding(InputKey.A, FluXisKeybind.Key7k1),
            new KeyBinding(InputKey.S, FluXisKeybind.Key7k2),
            new KeyBinding(InputKey.D, FluXisKeybind.Key7k3),
            new KeyBinding(InputKey.Space, FluXisKeybind.Key7k4),
            new KeyBinding(InputKey.J, FluXisKeybind.Key7k5),
            new KeyBinding(InputKey.K, FluXisKeybind.Key7k6),
            new KeyBinding(InputKey.L, FluXisKeybind.Key7k7),

            new KeyBinding(InputKey.Space, FluXisKeybind.Skip),
            new KeyBinding(InputKey.Shift, FluXisKeybind.Restart),
            new KeyBinding(InputKey.Control, FluXisKeybind.Exit),
            new KeyBinding(InputKey.Escape, FluXisKeybind.Pause),
            new KeyBinding(InputKey.Left, FluXisKeybind.SeekBackward),
            new KeyBinding(InputKey.Right, FluXisKeybind.SeekForward),

            new KeyBinding(new KeyCombination(InputKey.Control, InputKey.A), FluXisKeybind.ToggleAutoplay),
            new KeyBinding(new KeyCombination(InputKey.Control, InputKey.D), FluXisKeybind.ForceDeath),
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
        ToggleOnlineOverlay,

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

        // Ingame (Debug)
        ToggleAutoplay,
        ForceDeath,
    }
}
