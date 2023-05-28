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
        SetDefault(FluXisSetting.ScrollSpeed, 2f, 1f, 10f, 0.1f);
        SetDefault(FluXisSetting.HideFlawless, false);
        SetDefault(FluXisSetting.ShowEarlyLate, false);
        SetDefault(FluXisSetting.BackgroundPulse, false);
        SetDefault(FluXisSetting.HitErrorScale, 1f, 0.5f, 1.2f, 0.01f);
        SetDefault(FluXisSetting.LaneSwitchAlerts, true);

        SetDefault(FluXisSetting.BackgroundDim, 0.4f, 0f, 1f, 0.01f);
        SetDefault(FluXisSetting.BackgroundBlur, 0f, 0f, 1f, 0.01f);

        SetDefault(FluXisSetting.MainMenuVisualizer, true);
        SetDefault(FluXisSetting.SkipIntro, false);

        SetDefault(FluXisSetting.GlobalOffset, 0, -1000, 1000, 1);
        SetDefault(FluXisSetting.HitSoundVolume, 1d, 0d, 1d, 0.01d);

        SetDefault(FluXisSetting.Token, string.Empty);
    }
}

public enum FluXisSetting
{
    // Gameplay
    ScrollSpeed,
    HideFlawless,
    ShowEarlyLate,
    BackgroundPulse,
    HitErrorScale,
    LaneSwitchAlerts,

    // Graphics
    BackgroundDim,
    BackgroundBlur,

    // UI
    MainMenuVisualizer,
    SkipIntro,

    // Audio
    GlobalOffset,
    HitSoundVolume,

    // Account
    Token
}
