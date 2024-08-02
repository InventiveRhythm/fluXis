using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.Sections.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

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
