using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Plugins.Import;
using fluXis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Plugins;

public partial class PluginsImportSection : SettingsSubSection
{
    public override LocalisableString Title => LocalizationStrings.Settings.Plugins.ImportPlugins;
    public override IconUsage Icon => FontAwesome6.Solid.Plug;

    [BackgroundDependencyLoader]
    private void load(PluginManager plugins)
    {
        foreach (var plugin in plugins.Plugins.Where(p => p is { Importer: not null }))
            Add(new DrawableImportPlugin { Plugin = plugin });
    }
}
