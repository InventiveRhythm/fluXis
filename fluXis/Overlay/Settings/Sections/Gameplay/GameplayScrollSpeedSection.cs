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

public partial class GameplayScrollSpeedSection : SettingsSubSection
{
    public override LocalisableString Title => strings.ScrollSpeed;
    public override IconUsage Icon => FontAwesome6.Solid.UpDown;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = strings.ScrollSpeed,
                Description = strings.ScrollSpeedDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.ScrollSpeed),
                Step = 0.1f
            }
        });
    }
}
