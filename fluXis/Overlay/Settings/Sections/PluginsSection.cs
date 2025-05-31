using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class PluginsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.PuzzlePiece;
    public override LocalisableString Title => LocalizationStrings.Settings.Plugins.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new PluginsImportSection()
            // Divider,
            // new PluginsEditorSection(),
        });
    }
}
