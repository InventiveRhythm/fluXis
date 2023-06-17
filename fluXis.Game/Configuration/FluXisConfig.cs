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
        SetDefault(FluXisSetting.HitErrorScale, 1f, 0.5f, 1.2f, 0.01f);

        // Gameplay
        SetDefault(FluXisSetting.ScrollDirection, ScrollDirection.Down);
        SetDefault(FluXisSetting.SnapColoring, false);
        SetDefault(FluXisSetting.HideFlawless, false);
        SetDefault(FluXisSetting.ShowEarlyLate, false);
        SetDefault(FluXisSetting.JudgementSplash, true);
        SetDefault(FluXisSetting.LaneCoverTop, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.LaneCoverBottom, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.DimAndFade, true);

        SetDefault(FluXisSetting.ScrollSpeed, 2f, 1f, 10f, 0.1f);

        SetDefault(FluXisSetting.BackgroundDim, 0.4f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundBlur, 0f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundPulse, false);

        SetDefault(FluXisSetting.LaneSwitchAlerts, true);
        SetDefault(FluXisSetting.DisableEpilepsyIntrusingEffects, false);

        SetDefault(FluXisSetting.HudVisibility, HudVisibility.Always);

        // UI
        SetDefault(FluXisSetting.HoldToConfirm, 400f, 0f, 1000f, 200f);
        SetDefault(FluXisSetting.SkipIntro, false);
        SetDefault(FluXisSetting.Parallax, true);
        SetDefault(FluXisSetting.MainMenuVisualizer, true);
        SetDefault(FluXisSetting.SongSelectBlur, true);

        // Audio
        SetDefault(FluXisSetting.GlobalOffset, 0, -1000, 1000, 1);
        SetDefault(FluXisSetting.HitSoundVolume, 1d, 0d, 1d, 0.01d);

        // Graphics
        SetDefault(FluXisSetting.ShowFps, false);

        // Account
        SetDefault(FluXisSetting.Token, string.Empty);
    }
}

public enum FluXisSetting
{
    // Appearance
    SkinName,
    HitErrorScale,

    // Gameplay
    ScrollDirection,
    SnapColoring,
    HideFlawless,
    ShowEarlyLate,
    JudgementSplash,
    LaneCoverTop,
    LaneCoverBottom,
    DimAndFade,

    ScrollSpeed,

    BackgroundDim,
    BackgroundBlur,
    BackgroundPulse,

    LaneSwitchAlerts,
    DisableEpilepsyIntrusingEffects,

    HudVisibility,

    // UI
    HoldToConfirm,
    SkipIntro,
    Parallax,
    MainMenuVisualizer,
    SongSelectBlur,

    // Audio
    HitSoundVolume,
    GlobalOffset,

    // Graphics
    ShowFps,

    // Account
    Token
}
