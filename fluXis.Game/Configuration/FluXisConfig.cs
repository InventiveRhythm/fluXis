using System.Collections.Generic;
using fluXis.Game.Screens.Select.Info.Scores;
using fluXis.Game.Utils;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace fluXis.Game.Configuration;

public class FluXisConfig : IniConfigManager<FluXisSetting>
{
    public FluXisConfig(Storage storage, IDictionary<FluXisSetting, object> defaultOverrides = null)
        : base(storage, defaultOverrides)
    {
    }

    protected override void InitialiseDefaults()
    {
        // Appearance
        SetDefault(FluXisSetting.SkinName, "Default");
        SetDefault(FluXisSetting.LayoutName, "Default");

        // Gameplay
        SetDefault(FluXisSetting.ScrollDirection, ScrollDirection.Down);
        SetDefault(FluXisSetting.SnapColoring, false);
        SetDefault(FluXisSetting.TimingLines, true);
        SetDefault(FluXisSetting.HideFlawless, false);
        SetDefault(FluXisSetting.ShowEarlyLate, false);
        SetDefault(FluXisSetting.JudgementSplash, true);
        SetDefault(FluXisSetting.LaneCoverTop, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.LaneCoverBottom, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.DimAndFade, true);

        SetDefault(FluXisSetting.ScrollSpeed, 3f, 2f, 8f, 0.1f);

        SetDefault(FluXisSetting.Hitsounding, true);
        SetDefault(FluXisSetting.BackgroundVideo, true);

        SetDefault(FluXisSetting.BackgroundDim, 0.4f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundBlur, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundPulse, false);

        SetDefault(FluXisSetting.LaneSwitchAlerts, true);

        SetDefault(FluXisSetting.HudVisibility, HudVisibility.Always);
        SetDefault(FluXisSetting.GameplayLeaderboardVisible, true);
        SetDefault(FluXisSetting.GameplayLeaderboardMode, GameplayLeaderboardMode.Score);

        // UI
        SetDefault(FluXisSetting.UIScale, 1, 1f, 1.5f, 0.01f);
        SetDefault(FluXisSetting.HoldToConfirm, 400f, 0f, 1000f, 200f);
        SetDefault(FluXisSetting.SkipIntro, false);
        SetDefault(FluXisSetting.Parallax, true);

        // UI // Main Menu
        SetDefault(FluXisSetting.IntroTheme, true);
        SetDefault(FluXisSetting.ForceSnow, false);
        SetDefault(FluXisSetting.MainMenuVisualizer, true);
        SetDefault(FluXisSetting.MainMenuVisualizerSway, false);

        // UI // Song Select
        SetDefault(FluXisSetting.SongSelectBlur, true);
        SetDefault(FluXisSetting.LoopMode, LoopMode.Limited);
        SetDefault(FluXisSetting.GroupingMode, MapUtils.GroupingMode.Default);
        SetDefault(FluXisSetting.SortingMode, MapUtils.SortingMode.Title);
        SetDefault(FluXisSetting.SortingInverse, false);
        SetDefault(FluXisSetting.LeaderboardType, ScoreListType.Local);

        // UI // Editor
        SetDefault(FluXisSetting.EditorDim, 0.4f, 0f, .8f, 0.2f);
        SetDefault(FluXisSetting.EditorBlur, 0f, 0f, 1f, 0.2f);
        SetDefault(FluXisSetting.EditorShowSamples, false);

        // Audio
        SetDefault(FluXisSetting.InactiveVolume, 0.5d, 0d, 1d, 0.01d);
        SetDefault(FluXisSetting.HitSoundVolume, 1d, 0d, 1d, 0.01d);

        // Audio // Panning
        SetDefault(FluXisSetting.UIPanning, 0.75d, 0d, 1d, 0.01d);
        SetDefault(FluXisSetting.HitsoundPanning, 1d, 0d, 1d, 0.01d);

        // Audio // Offset
        SetDefault(FluXisSetting.GlobalOffset, 0, -1000, 1000, 1);
        SetDefault(FluXisSetting.DisableOffsetInReplay, true);

        // Graphics
        SetDefault(FluXisSetting.ShowFps, false);

        // Account
        SetDefault(FluXisSetting.Username, string.Empty);
        SetDefault(FluXisSetting.Token, string.Empty);

        // Debug
        SetDefault(FluXisSetting.LogAPIResponses, false);

        // Misc
        SetDefault(FluXisSetting.ReleaseChannel, ReleaseChannel.Stable);
    }
}

public enum FluXisSetting
{
    // Appearance
    SkinName,
    LayoutName,

    // Gameplay
    ScrollDirection,
    SnapColoring,
    TimingLines,
    HideFlawless,
    ShowEarlyLate,
    JudgementSplash,
    LaneCoverTop,
    LaneCoverBottom,
    DimAndFade,

    ScrollSpeed,

    Hitsounding,
    BackgroundVideo,

    BackgroundDim,
    BackgroundBlur,
    BackgroundPulse,

    LaneSwitchAlerts,

    HudVisibility,
    GameplayLeaderboardVisible,
    GameplayLeaderboardMode,

    // UI
    UIScale,
    HoldToConfirm,
    SkipIntro,
    Parallax,

    // UI // Main Menu
    IntroTheme,
    ForceSnow,
    MainMenuVisualizer,
    MainMenuVisualizerSway,

    // UI // Song Select
    SongSelectBlur,
    LoopMode,
    GroupingMode,
    SortingMode,
    SortingInverse,
    LeaderboardType,

    // UI // Editor
    EditorDim,
    EditorBlur,
    EditorShowSamples,

    // Audio
    InactiveVolume,
    HitSoundVolume,

    // Audio // Panning
    UIPanning,
    HitsoundPanning,

    // Audio // Offset
    GlobalOffset,
    DisableOffsetInReplay,

    // Graphics
    ShowFps,

    // Account
    Username,
    Token,

    // Debug
    LogAPIResponses,

    // Misc
    ReleaseChannel
}
