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

public partial class GameplayMapSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Map;
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.Hitsounds,
                Description = strings.HitsoundsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.Hitsounding)
            },
            new SettingsToggle
            {
                Label = strings.BackgroundVideo,
                Description = strings.BackgroundVideoDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.BackgroundVideo)
            },
        });
    }
}
