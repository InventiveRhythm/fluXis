using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class AudioSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.VolumeUp;
    public override string Title => "Audio";
}
