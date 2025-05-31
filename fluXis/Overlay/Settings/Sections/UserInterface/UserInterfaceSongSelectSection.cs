using System;
using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceSongSelectSection : SettingsSubSection
{
    public override LocalisableString Title => strings.SongSelect;
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    private SettingsUIStrings strings => LocalizationStrings.Settings.UI;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.SongSelectBlur,
                Description = strings.SongSelectBlurDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.SongSelectBlur)
            },
            new SettingsDropdown<LoopMode>
            {
                Label = strings.LoopMode,
                Description = strings.LoopModeDescription,
                Items = Enum.GetValues<LoopMode>(),
                Bindable = Config.GetBindable<LoopMode>(FluXisSetting.LoopMode)
            }
        });
    }
}
