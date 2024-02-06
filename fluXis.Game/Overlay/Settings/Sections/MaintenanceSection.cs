using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.Sections.Maintenance;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class MaintenanceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.ScrewdriverWrench;
    public override LocalisableString Title => LocalizationStrings.Settings.Maintenance.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new[]
        {
            new MaintenanceFilesSection()
        });
    }
}
