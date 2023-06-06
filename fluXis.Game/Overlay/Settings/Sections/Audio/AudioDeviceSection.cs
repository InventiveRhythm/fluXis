using System.Linq;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioDeviceSection : SettingsSubSection
{
    public override string Title => "Device";

    [Resolved]
    private AudioManager audio { get; set; }

    private SettingsDropdown<string> deviceDropdown;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(deviceDropdown = new SettingsDropdown<string>
        {
            Label = "Output Device",
            Bindable = audio.AudioDevice,
            Items = audio.AudioDeviceNames.Where(i => i != null).Distinct().ToList()
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
