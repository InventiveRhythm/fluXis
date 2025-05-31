using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Maintenance;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class MaintenanceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.ScrewdriverWrench;
    public override LocalisableString Title => LocalizationStrings.Settings.Maintenance.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new MaintenanceFilesSection(),
            Divider,
            new MaintenanceMapsSection()
        });
    }
}
