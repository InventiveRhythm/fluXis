using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Gameplay;

public partial class GameplayEffectsSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Effects;
    public override IconUsage Icon => FontAwesome6.Solid.Star;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.LaneSwitchAlerts,
                Description = strings.LaneSwitchAlertsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.LaneSwitchAlerts)
            },
            new SettingsToggle
            {
                Label = strings.DisableBloom,
                Description = strings.DisableBloomDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.DisableBloom)
            }
        });
    }
}
