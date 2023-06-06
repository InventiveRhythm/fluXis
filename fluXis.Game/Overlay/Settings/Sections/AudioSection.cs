using fluXis.Game.Overlay.Settings.Sections.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class AudioSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.VolumeUp;
    public override string Title => "Audio";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new AudioDeviceSection(),
            Divider,
            new AudioVolumeSection(),
            Divider,
            new AudioOffsetSection()
        });
    }
}
