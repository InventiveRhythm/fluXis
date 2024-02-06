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
                Label = strings.DisableEpilepsyIntrusingEffects,
                Description = strings.DisableEpilepsyIntrusingEffectsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.DisableEpilepsyIntrusingEffects)
            }
        });
    }
}
