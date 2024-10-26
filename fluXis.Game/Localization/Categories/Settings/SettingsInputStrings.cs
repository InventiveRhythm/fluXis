using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsInputStrings : LocalizationCategory
{
    protected override string File => "settings-input";

    public TranslatableString Title => Get("title", "Input");

    #region Keybindings

    public TranslatableString Keybindings => Get("keybindings-title", "Keybindings");

    public TranslatableString Navigation => Get("keybindings-navigation", "Navigation");

    public TranslatableString PreviousSelection => Get("keybindings-previous-selection", "Previous Selection");
    public TranslatableString NextSelection => Get("keybindings-next-selection", "Next Selection");
    public TranslatableString PreviousGroup => Get("keybindings-previous-group", "Previous Group");
    public TranslatableString NextGroup => Get("keybindings-next-group", "Next Group");
    public TranslatableString Back => Get("keybindings-back", "Back");
    public TranslatableString Select => Get("keybindings-select", "Select");

    public TranslatableString SongSelect => Get("keybindings-song-select", "Song Select");

    public TranslatableString DecreaseRate => Get("keybindings-decrease-rate", "Decrease Rate");
    public TranslatableString IncreaseRate => Get("keybindings-increase-rate", "Increase Rate");

    public TranslatableString Editing => Get("keybindings-editing", "Editing");

    public TranslatableString DeleteSelection => Get("keybindings-delete-selection", "Delete Selection");
    public TranslatableString Undo => Get("keybindings-undo", "Undo");
    public TranslatableString Redo => Get("keybindings-redo", "Redo");

    public TranslatableString Overlays => Get("keybindings-overlays", "Overlays");

    public TranslatableString ToggleSettings => Get("keybindings-toggle-settings", "Toggle Settings");

    public TranslatableString Audio => Get("keybindings-audio", "Audio");

    public TranslatableString VolumeDown => Get("keybindings-volume-down", "Decrease Volume");
    public TranslatableString VolumeUp => Get("keybindings-volume-up", "Increase Volume");
    public TranslatableString PreviousVolumeCategory => Get("keybindings-previous-category", "Previous Category");
    public TranslatableString NextVolumeCategory => Get("keybindings-next-category", "Next Category");
    public TranslatableString PreviousTrack => Get("keybindings-previous-track", "Previous Track");
    public TranslatableString NextTrack => Get("keybindings-next-track", "Next Track");
    public TranslatableString PlayPause => Get("keybindings-play-pause", "Play/Pause Track");

    public TranslatableString Keymodes => Get("keybindings-keymodes", "Keymodes");
    public TranslatableString OtherKeymodes => Get("keybindings-other-keymodes", "Other Keymodes");

    public TranslatableString KeymodesDual => Get("keybindings-keymodes-dual", "Keymodes (Dual)");
    public TranslatableString OtherKeymodesDual => Get("keybindings-other-keymodes-dual", "Other Keymodes (Dual)");

    public TranslatableString OneKey => Get("keybindings-1k", "1 Key Layout");
    public TranslatableString TwoKey => Get("keybindings-2k", "2 Key Layout");
    public TranslatableString ThreeKey => Get("keybindings-3k", "3 Key Layout");
    public TranslatableString FourKey => Get("keybindings-4k", "4 Key Layout");
    public TranslatableString FiveKey => Get("keybindings-5k", "5 Key Layout");
    public TranslatableString SixKey => Get("keybindings-6k", "6 Key Layout");
    public TranslatableString SevenKey => Get("keybindings-7k", "7 Key Layout");
    public TranslatableString EightKey => Get("keybindings-8k", "8 Key Layout");
    public TranslatableString NineKey => Get("keybindings-9k", "9 Key Layout");
    public TranslatableString TenKey => Get("keybindings-10k", "10 Key Layout");

    public TranslatableString InGame => Get("keybindings-in-game", "In-Game");

    public TranslatableString SkipIntro => Get("keybindings-skip-intro", "Skip Intro");
    public TranslatableString DecreaseSpeed => Get("keybindings-decrease-speed", "Decrease Scroll Speed");
    public TranslatableString IncreaseSpeed => Get("keybindings-increase-speed", "Increase Scroll Speed");
    public TranslatableString QuickRestart => Get("keybindings-quick-restart", "Quick Restart");
    public TranslatableString QuickExit => Get("keybindings-quick-exit", "Quick Exit");
    public TranslatableString SeekBackward => Get("keybindings-seek-backward", "Seek Backward");
    public TranslatableString SeekForward => Get("keybindings-seek-forward", "Seek Forward");

    #endregion

    #region Mouse

    public TranslatableString Mouse => Get("mouse-title", "Mouse");

    public TranslatableString HighPrecisionMouse => Get("mouse-high-precision", "High Precision Mouse");
    public TranslatableString HighPrecisionMouseDescription => Get("mouse-high-precision-description", "Uses raw input from the mouse instead of the cursor position.");

    public TranslatableString Sensitivity => Get("mouse-sensitivity", "Sensitivity");
    public TranslatableString SensitivityDescription => Get("mouse-sensitivity-description", "This is only used when High Precision Mouse is enabled.");

    #endregion
}
