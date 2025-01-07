using System;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.General;

public partial class GeneralUpdatesSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Updates;
    public override IconUsage Icon => FontAwesome6.Solid.Rotate;

    private SettingsGeneralStrings strings => LocalizationStrings.Settings.General;

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<ReleaseChannel>
            {
                Label = strings.ReleaseChannel,
                Description = strings.ReleaseChannelDescription,
                Bindable = Config.GetBindable<ReleaseChannel>(FluXisSetting.ReleaseChannel),
                Items = Enum.GetValues<ReleaseChannel>()
            },
            new SettingsButton
            {
                Label = strings.UpdatesCheck,
                Description = strings.UpdatesCheckDescription,
                ButtonText = "Check",
                Action = () => game.PerformUpdateCheck(false)
            }
        });
    }
}
