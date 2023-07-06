using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Maintenance;

public partial class MaintenanceFilesSection : SettingsSubSection
{
    public override string Title => "Files";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = "Clean up files",
                Enabled = false,
                ButtonText = "Run"
            }
        });
    }
}
