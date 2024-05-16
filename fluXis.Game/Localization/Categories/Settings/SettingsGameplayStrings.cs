using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsGameplayStrings : LocalizationCategory
{
    protected override string File => "settings-gameplay";

    public TranslatableString Title => Get("title", "Gameplay");

    #region General

    public TranslatableString General => Get("general-title", "General");

    public TranslatableString ScrollDirection => Get("scroll-direction", "Scroll Direction");
    public TranslatableString ScrollDirectionDescription => Get("scroll-direction-description", "The direction in which notes scroll.");

    public TranslatableString SnapColoring => Get("snap-coloring", "Snap Coloring");
    public TranslatableString SnapColoringDescription => Get("snap-coloring-description", "Color notes based on their snap divisor.");

    public TranslatableString TimingLines => Get("timing-lines", "Timing Lines");
    public TranslatableString TimingLinesDescription => Get("timing-lines-description", "Show a line every 4 beats.");

    public TranslatableString HideFlawless => Get("hide-flawless", "Hide Flawless Judgement");
    public TranslatableString HideFlawlessDescription => Get("hide-flawless-description", "Hides the best judgement (Flawless) from popping up during gameplay.");

    public TranslatableString ShowEarlyLate => Get("show-early-late", "Show Early/Late");
    public TranslatableString ShowEarlyLateDescription => Get("show-early-late-description", "Show early/late below the judgement.");

    public TranslatableString JudgementSplash => Get("judgement-splash", "Judgement Splash");
    public TranslatableString JudgementSplashDescription => Get("judgement-splash-description", "Show a splash when a judgement is hit.");

    public TranslatableString LaneCoverTop => Get("lane-cover-top", "Top Lane Cover");
    public TranslatableString LaneCoverTopDescription => Get("lane-cover-top-description", "How much the top of the playfield is covered.");

    public TranslatableString LaneCoverBottom => Get("lane-cover-bottom", "Bottom Lane Cover");
    public TranslatableString LaneCoverBottomDescription => Get("lane-cover-bottom-description", "How much the bottom of the playfield is covered.");

    public TranslatableString HealthEffects => Get("health-effects", "Health Effects");
    public TranslatableString HealthEffectsDescription => Get("health-effects-description", "Dims and fades the entire playfield when health is low.");

    #endregion

    #region Scroll Speed

    public TranslatableString ScrollSpeed => Get("scroll-speed", "Scroll Speed");
    public TranslatableString ScrollSpeedDescription => Get("scroll-speed-description", "The speed at which notes move across the screen.");

    #endregion

    #region Background

    public TranslatableString Background => Get("background-title", "Background");

    public TranslatableString BackgroundDim => Get("background-dim", "Background Dim");
    public TranslatableString BackgroundDimDescription => Get("background-dim-description", "How much the background is dimmed.");

    public TranslatableString BackgroundBlur => Get("background-blur", "Background Blur");
    public TranslatableString BackgroundBlurDescription => Get("background-blur-description", "How much the background is blurred.");

    public TranslatableString BackgroundPulse => Get("background-pulse", "Background Pulse");
    public TranslatableString BackgroundPulseDescription => Get("background-pulse-description", "Pulse the background to the beat of the music.");

    #endregion

    #region Map

    public TranslatableString Map => Get("map-title", "Map");

    public TranslatableString Hitsounds => Get("map-hitsounds", "Map Hitsounds");
    public TranslatableString HitsoundsDescription => Get("map-hitsounds-description", "Use the map's custom hitsounds.");

    public TranslatableString BackgroundVideo => Get("background-video", "Video / Storyboard");
    public TranslatableString BackgroundVideoDescription => Get("background-video-description", "Show the video or storyboard in the background.");

    #endregion

    #region Effects

    public TranslatableString Effects => Get("effects-title", "Effects");

    public TranslatableString LaneSwitchAlerts => Get("lane-switch-alerts", "Lane Switch Alerts");
    public TranslatableString LaneSwitchAlertsDescription => Get("lane-switch-alerts-description", "Shows arrows next to the playfield before a lane switch.");

    #endregion

    #region HUD

    public TranslatableString HUD => Get("hud-title", "HUD");

    public TranslatableString Visibility => Get("hud-visibility", "Visibility");
    public TranslatableString VisibilityDescription => Get("hud-visibility-description", "When the HUD should be visible.");

    #endregion
}
