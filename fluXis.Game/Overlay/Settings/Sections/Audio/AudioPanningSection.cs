using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioPanningSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Panning;
    public override IconUsage Icon => FontAwesome6.Solid.LeftRight;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<double>
            {
                Label = strings.UIPanning,
                Description = strings.UIPanningDescription,
                Bindable = Config.GetBindable<double>(FluXisSetting.UIPanning),
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = strings.HitsoundPanning,
                Description = strings.HitsoundPanningDescription,
                Bindable = Config.GetBindable<double>(FluXisSetting.HitsoundPanning),
                DisplayAsPercentage = true
            }
        });
    }
}
