using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class AudioSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.VolumeHigh;
    public override LocalisableString Title => LocalizationStrings.Settings.Audio.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new AudioDeviceSection(),
            Divider,
            new AudioVolumeSection(),
            Divider,
            new AudioOffsetSection(),
            Divider,
            new AudioPanningSection()
        });
    }
}
