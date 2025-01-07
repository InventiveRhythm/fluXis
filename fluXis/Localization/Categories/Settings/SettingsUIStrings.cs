using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsUIStrings : LocalizationCategory
{
    protected override string File => "settings-ui";

    public TranslatableString Title => Get("title", "User Interface");

    #region General

    public TranslatableString General => Get("general-title", "General");

    public TranslatableString UIScale => Get("ui-scale", "UI Scale");
    public TranslatableString UIScaleDescription => Get("ui-scale-description", "Adjust the size of the interface.");

    public TranslatableString ConfirmDuration => Get("confirm-duration", "Hold to Confirm Duration");
    public TranslatableString ConfirmDurationDescription => Get("confirm-duration-description", "How long to hold a button to confirm an action.");

    public TranslatableString SkipWarning => Get("skip-warning", "Skip Warning");
    public TranslatableString SkipWarningDescription => Get("skip-warning-description", "Skips the epilepsy warning at the start of the game.");

    public TranslatableString Parallax => Get("parallax", "Parallax");
    public TranslatableString ParallaxDescription => Get("parallax-description", "Enable the parallax effect on backgrounds and such.");

    #endregion

    #region Main Menu

    public TranslatableString MainMenu => Get("main-menu-title", "Main Menu");

    public TranslatableString IntroMusic => Get("intro-music", "fluXis intro music");
    public TranslatableString IntroMusicDescription => Get("intro-music-description", "Play the fluXis intro music on startup. Disabling this will play a random song from your library instead.");

    public TranslatableString ForceSnow => Get("force-snow", "Force Snow");
    public TranslatableString ForceSnowDescription => Get("force-snow-description", "Makes the snow always visible, not just in the winter.");

    public TranslatableString BubbleVisualizer => Get("bubble-visualizer", "Bubble Visualizer");
    public TranslatableString BubbleVisualizerDescription => Get("bubble-visualizer-description", "Enable the bubble visualizer on the main menu.");

    public TranslatableString BubbleSway => Get("bubble-sway", "Bubble Sway");
    public TranslatableString BubbleSwayDescription => Get("bubble-sway-description", "Moves the bubbles horizontally.");

    #endregion

    #region Song Select

    public TranslatableString SongSelect => Get("song-select-title", "Song Select");

    public TranslatableString SongSelectBlur => Get("song-select-blur", "Blur Background");
    public TranslatableString SongSelectBlurDescription => Get("song-select-blur-description", "Blur the background of the song select screen.");

    public TranslatableString LoopMode => Get("loop-mode", "Loop Mode");
    public TranslatableString LoopModeDescription => Get("loop-mode-description", "How the song select music should loop.");

    #endregion
}
