using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.UserInterface;

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
