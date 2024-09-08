using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioDeviceSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Device;
    public override IconUsage Icon => FontAwesome6.Solid.Headphones;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [Resolved]
    private AudioManager audio { get; set; }

    private AudioDropdown deviceDropdown;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(deviceDropdown = new AudioDropdown
        {
            Label = strings.OutputDevice
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
