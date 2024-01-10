using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioDeviceSection : SettingsSubSection
{
    public override string Title => "Device";
    public override IconUsage Icon => FontAwesome.Solid.Headphones;

    [Resolved]
    private AudioManager audio { get; set; }

    private AudioDropdown deviceDropdown;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(deviceDropdown = new AudioDropdown
        {
            Label = "Output Device"
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

        private partial class AudioDropdownMenu : SettingsDropdownMenu
        {
            protected override LocalisableString GenerateItemText(string item)
                => string.IsNullOrEmpty(item) ? "Default" : base.GenerateItemText(item);
        }
    }
}
