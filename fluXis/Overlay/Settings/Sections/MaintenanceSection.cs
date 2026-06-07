using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Advanced;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class MaintenanceSection : SettingsSection
{
    public override IconUsage Icon => Phosphor.Bold.Wrench;
    public override LocalisableString Title => LocalizationStrings.Settings.Advanced.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new AdvancedFilesSection(),
            Divider,
            new AdvancedMapsSection(),
            Divider,
            new AdvancedDebugSection()
        });
    }
}
