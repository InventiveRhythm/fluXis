using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class AudioSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.VolumeUp;
    public override string Title => "Audio";

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        Add(new SettingsSlider<double>(audio.Volume, "Master Volume", "", true));
        Add(new SettingsSlider<double>(audio.VolumeTrack, "Music Volume", "", true));
        Add(new SettingsSlider<double>(audio.VolumeSample, "Effect Volume", "", true));
    }
}
