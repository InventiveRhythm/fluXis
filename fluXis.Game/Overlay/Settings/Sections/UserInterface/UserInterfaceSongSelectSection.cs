using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceSongSelectSection : SettingsSubSection
{
    public override string Title => "Song Select";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Blur Background",
                Enabled = false,
                Bindable = Config.GetBindable<bool>(FluXisSetting.SongSelectBlur)
            }
        });
    }
}
