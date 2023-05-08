using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class AudioSection : SettingsSection
{
    [Resolved]
    private AudioManager audio { get; set; }

    public override IconUsage Icon => FontAwesome.Solid.VolumeUp;
    public override string Title => "Audio";

    private SettingsDropdown<string> deviceDropdown;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio, FluXisConfig config)
    {
        AddRange(new SettingsItem[]
        {
            deviceDropdown = new SettingsDropdown<string>
            {
                Label = "Output Device",
                Bindable = audio.AudioDevice,
                Items = audio.AudioDeviceNames.Where(i => i != null).Distinct().ToList()
            },
            new SettingsSlider<double>
            {
                Label = "Master Volume",
                Bindable = audio.Volume,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = "Music Volume",
                Bindable = audio.VolumeTrack,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = "Effect Volume",
                Bindable = audio.VolumeSample,
                DisplayAsPercentage = true
            },
            new SettingsSlider<double>
            {
                Label = "Hit Sound Volume",
                Bindable = config.GetBindable<double>(FluXisSetting.HitSoundVolume),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = "Global Offset",
                ValueLabel = "{value}ms",
                Bindable = config.GetBindable<float>(FluXisSetting.GlobalOffset),
                Step = 1
            }
        });

        audio.OnNewDevice += _ => updateDeviceDropdown();
        audio.OnLostDevice += _ => updateDeviceDropdown();
    }

    private void updateDeviceDropdown()
    {
        deviceDropdown.Bindable = audio.AudioDevice;
        deviceDropdown.Items = audio.AudioDeviceNames.Where(i => i != null).Distinct().ToList();
    }
}
