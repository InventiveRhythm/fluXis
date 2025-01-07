using System;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Gameplay;

public partial class GameplayHudSection : SettingsSubSection
{
    public override LocalisableString Title => strings.HUD;
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<HudVisibility>
            {
                Label = strings.Visibility,
                Description = strings.VisibilityDescription,
                Items = Enum.GetValues<HudVisibility>(),
                Bindable = Config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility)
            },
            new SettingsToggle
            {
                Label = strings.LeaderboardVisibility,
                Description = strings.LeaderboardVisibilityDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.GameplayLeaderboardVisible)
            },
            new SettingsDropdown<GameplayLeaderboardMode>
            {
                Label = strings.LeaderboardMode,
                Description = strings.LeaderboardModeDescription,
                Items = Enum.GetValues<GameplayLeaderboardMode>(),
                Bindable = Config.GetBindable<GameplayLeaderboardMode>(FluXisSetting.GameplayLeaderboardMode)
            }
        });
    }
}
