using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayGeneralSection : SettingsSubSection
{
    public override LocalisableString Title => strings.General;
    public override IconUsage Icon => FontAwesome6.Solid.Gear;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<ScrollDirection>
            {
                Label = strings.ScrollDirection,
                Description = strings.ScrollDirectionDescription,
                Items = Enum.GetValues<ScrollDirection>(),
                Bindable = Config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection)
            },
            new SettingsToggle
            {
                Label = strings.SnapColoring,
                Description = strings.SnapColoringDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.SnapColoring)
            },
            new SettingsToggle
            {
                Label = strings.TimingLines,
                Description = strings.TimingLinesDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.TimingLines)
            },
            new SettingsToggle
            {
                Label = strings.HideFlawless,
                Description = strings.HideFlawlessDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.HideFlawless)
            },
            new SettingsToggle
            {
                Label = strings.ShowEarlyLate,
                Description = strings.ShowEarlyLateDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowEarlyLate)
            },
            new SettingsToggle
            {
                Label = strings.JudgementSplash,
                Description = strings.JudgementSplashDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.JudgementSplash)
            },
            new SettingsSlider<float>
            {
                Label = strings.LaneCoverTop,
                Description = strings.LaneCoverTopDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.LaneCoverTop),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = strings.LaneCoverBottom,
                Description = strings.LaneCoverBottomDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.LaneCoverBottom),
                DisplayAsPercentage = true
            },
            new SettingsToggle
            {
                Label = strings.HealthEffects,
                Description = strings.HealthEffectsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.DimAndFade)
            }
        });
    }
}
