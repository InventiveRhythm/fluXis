using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Audio;

public partial class AudioDeviceSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Device;
    public override IconUsage Icon => FontAwesome6.Solid.Headphones;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [Resolved]
    private AudioManager audio { get; set; }

    private AudioDropdown deviceDropdown;

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager fwConfig)
    {
        Add(deviceDropdown = new AudioDropdown
        {
            Label = strings.OutputDevice
        });

        Add(new SettingsToggle
        {
            Label = "Use WASAPI (Experimental)",
            Description = "Makes the audio library use WASAPI on Windows. Experimental and needs adjustment to offset after changing.",
            Bindable = fwConfig.GetBindable<bool>(FrameworkSetting.AudioUseExperimentalWasapi)
        });

        updateDeviceDropdown();

        audio.OnNewDevice += onDeviceChanged;
        audio.OnLostDevice += onDeviceChanged;
        deviceDropdown.Bindable = audio.AudioDevice;
    }

    private void onDeviceChanged(string name) => updateDeviceDropdown();

    private void updateDeviceDropdown()
    {
        var deviceItems = new List<string> { string.Empty };
        deviceItems.AddRange(audio.AudioDeviceNames);

        deviceDropdown.Items = deviceItems
                               .Where(i => i != null)
                               .Distinct()
                               .ToList();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        audio.OnNewDevice -= onDeviceChanged;
        audio.OnLostDevice -= onDeviceChanged;
    }

    private partial class AudioDropdown : SettingsDropdown<string>
    {
        protected override Dropdown<string> CreateMenu() => new AudioDropdownMenu();

        private partial class AudioDropdownMenu : CustomDropdown
        {
            protected override LocalisableString GenerateItemText(string item)
                => string.IsNullOrEmpty(item) ? "Default" : base.GenerateItemText(item);
        }
    }
}
