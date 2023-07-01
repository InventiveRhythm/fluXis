using System;
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
                Bindable = Config.GetBindable<bool>(FluXisSetting.SongSelectBlur)
            },
            new SettingsDropdown<LoopMode>
            {
                Label = "Loop Mode",
                Description = "How the song select music should loop.",
                Items = Enum.GetValues<LoopMode>(),
                Bindable = Config.GetBindable<LoopMode>(FluXisSetting.LoopMode)
            }
        });
    }
}
