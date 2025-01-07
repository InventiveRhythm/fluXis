using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Audio;

public partial class AudioVolumeSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Volume;
    public override IconUsage Icon => FontAwesome6.Solid.VolumeHigh;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio, FluXisConfig config)
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<double>
            {
                Label = strings.MasterVolume,
                Bindable = audio.Volume,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = strings.MasterVolumeInactive,
                Description = strings.MasterVolumeInactiveDescription,
                Bindable = config.GetBindable<double>(FluXisSetting.InactiveVolume),
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = strings.MusicVolume,
                Bindable = audio.VolumeTrack,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = strings.EffectVolume,
                Bindable = audio.VolumeSample,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = strings.HitSoundVolume,
                Bindable = Config.GetBindable<double>(FluXisSetting.HitSoundVolume),
                DisplayAsPercentage = true
            }
        });
    }
}
