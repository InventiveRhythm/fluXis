using System.Collections.Generic;
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

        SetDefault(FluXisSetting.ScrollSpeed, 2f, 1f, 10f, 0.1f);

        SetDefault(FluXisSetting.Hitsounding, true);
        SetDefault(FluXisSetting.BackgroundVideo, true);

        SetDefault(FluXisSetting.BackgroundDim, 0.4f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundBlur, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundPulse, false);

        SetDefault(FluXisSetting.LaneSwitchAlerts, true);
        SetDefault(FluXisSetting.DisableEpilepsyIntrusingEffects, false);

        SetDefault(FluXisSetting.HudVisibility, HudVisibility.Always);

        // UI
        SetDefault(FluXisSetting.UIScale, 1, 1f, 1.5f, 0.01f);
        SetDefault(FluXisSetting.HoldToConfirm, 400f, 0f, 1000f, 200f);
        SetDefault(FluXisSetting.SkipIntro, false);
        SetDefault(FluXisSetting.Parallax, true);
        SetDefault(FluXisSetting.MainMenuVisualizer, true);
        SetDefault(FluXisSetting.MainMenuVisualizerSway, false);

        // UI // Song Select
        SetDefault(FluXisSetting.SongSelectBlur, true);
        SetDefault(FluXisSetting.LoopMode, LoopMode.Limited);

        // UI // Editor
        SetDefault(FluXisSetting.EditorDim, 0.4f, 0f, .8f, 0.2f);
        SetDefault(FluXisSetting.EditorBlur, 0f, 0f, 1f, 0.2f);

        // Audio
        SetDefault(FluXisSetting.GlobalOffset, 0, -1000, 1000, 1);
        SetDefault(FluXisSetting.HitSoundVolume, 1d, 0d, 1d, 0.01d);

        // Graphics
        SetDefault(FluXisSetting.ShowFps, false);

        // Account
        SetDefault(FluXisSetting.Token, string.Empty);

        // Misc
        SetDefault(FluXisSetting.NowPlaying, false);
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
    DisableEpilepsyIntrusingEffects,

    HudVisibility,

    // UI
    UIScale,
    HoldToConfirm,
    SkipIntro,
    Parallax,
    MainMenuVisualizer,
    MainMenuVisualizerSway,

    // UI // Song Select
    SongSelectBlur,
    LoopMode,

    // UI // Editor
    EditorDim,
    EditorBlur,

    // Audio
    HitSoundVolume,
    GlobalOffset,

    // Graphics
    ShowFps,

    // Account
    Token,

    // Misc
    NowPlaying, // saves the current song to a json file
}
