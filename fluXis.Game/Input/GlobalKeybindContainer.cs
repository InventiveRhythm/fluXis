using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game.Input;

public partial class GlobalKeybindContainer : RealmKeyBindingContainer<FluXisGlobalKeybind>, IHandleGlobalKeyboardInput, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private readonly IKeyBindingHandler<FluXisGlobalKeybind> handler;

    public GlobalKeybindContainer(FluXisGameBase game, FluXisRealm realm)
        : base(realm, SimultaneousBindingMode.None, KeyCombinationMatchingMode.Modifiers)
    {
        if (game is IKeyBindingHandler<FluXisGlobalKeybind> keyBindingHandler)
            handler = keyBindingHandler;
    }

    public override IEnumerable<IKeyBinding> DefaultKeyBindings => GlobalKeyBindings
        .Concat(InGameKeyBindings);

    protected override bool Prioritised => true;

    public IEnumerable<KeyBinding> GlobalKeyBindings = new[]
    {
        new KeyBinding(InputKey.Enter, FluXisGlobalKeybind.Select),
        new KeyBinding(InputKey.Escape, FluXisGlobalKeybind.Back),
        new KeyBinding(InputKey.ExtraMouseButton1, FluXisGlobalKeybind.Back),
        new KeyBinding(InputKey.Up, FluXisGlobalKeybind.Previous),
        new KeyBinding(InputKey.Down, FluXisGlobalKeybind.Next),
        new KeyBinding(InputKey.Left, FluXisGlobalKeybind.PreviousGroup),
        new KeyBinding(InputKey.Right, FluXisGlobalKeybind.NextGroup),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.O), FluXisGlobalKeybind.ToggleSettings),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Shift, InputKey.S), FluXisGlobalKeybind.OpenSkinEditor),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.D), FluXisGlobalKeybind.ToggleDashboard),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.H), FluXisGlobalKeybind.Home),
        new KeyBinding(InputKey.Delete, FluXisGlobalKeybind.Delete),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Z), FluXisGlobalKeybind.Undo),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Y), FluXisGlobalKeybind.Redo),
        new KeyBinding(InputKey.F5, FluXisGlobalKeybind.MusicPrevious),
        new KeyBinding(InputKey.F6, FluXisGlobalKeybind.MusicPause),
        new KeyBinding(InputKey.F7, FluXisGlobalKeybind.MusicNext),
        new KeyBinding(InputKey.F8, FluXisGlobalKeybind.ToggleMusicPlayer),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Minus), FluXisGlobalKeybind.DecreaseRate),
        new KeyBinding(new KeyCombination(InputKey.Control, InputKey.Plus), FluXisGlobalKeybind.IncreaseRate),
        new KeyBinding(new KeyCombination(InputKey.Alt, InputKey.Left), FluXisGlobalKeybind.VolumeDecrease),
        new KeyBinding(new KeyCombination(InputKey.Alt, InputKey.Right), FluXisGlobalKeybind.VolumeIncrease),
        new KeyBinding(new KeyCombination(InputKey.Alt, InputKey.Up), FluXisGlobalKeybind.VolumePreviousCategory),
        new KeyBinding(new KeyCombination(InputKey.Alt, InputKey.Down), FluXisGlobalKeybind.VolumeNextCategory),
    };

    public IEnumerable<KeyBinding> InGameKeyBindings = new[]
    {
        new KeyBinding(InputKey.Space, FluXisGlobalKeybind.Skip),
        new KeyBinding(InputKey.Shift, FluXisGlobalKeybind.QuickRestart),
        new KeyBinding(InputKey.Control, FluXisGlobalKeybind.QuickExit),
        new KeyBinding(InputKey.Escape, FluXisGlobalKeybind.GameplayPause),
        new KeyBinding(InputKey.Left, FluXisGlobalKeybind.SeekBackward),
        new KeyBinding(InputKey.Right, FluXisGlobalKeybind.SeekForward),
        new KeyBinding(InputKey.F3, FluXisGlobalKeybind.ScrollSpeedDecrease),
        new KeyBinding(InputKey.F4, FluXisGlobalKeybind.ScrollSpeedIncrease),
    };

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e) => handler.OnPressed(e);
    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) => handler.OnReleased(e);
}

public enum FluXisGlobalKeybind
{
    // Global UI Keybinds
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
    Undo,
    Redo,
    MusicPrevious,
    MusicPause,
    MusicNext,
    ToggleMusicPlayer,

    // SongSelect Keybinds
    DecreaseRate,
    IncreaseRate,

    // Volume Keybinds
    VolumeDecrease,
    VolumeIncrease,
    VolumePreviousCategory,
    VolumeNextCategory,

    // In-game Keybinds
    Skip,
    QuickRestart,
    QuickExit,
    GameplayPause,
    SeekBackward,
    SeekForward,
    ScrollSpeedIncrease,
    ScrollSpeedDecrease
}
