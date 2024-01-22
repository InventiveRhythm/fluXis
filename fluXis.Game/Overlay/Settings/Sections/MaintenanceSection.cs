using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.Sections.Maintenance;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class MaintenanceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.ScrewdriverWrench;
    public override string Title => "Maintenance";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new[]
        {
            new MaintenanceFilesSection()
        });
    }
}
