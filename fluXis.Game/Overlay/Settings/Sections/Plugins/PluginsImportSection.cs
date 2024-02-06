using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.Sections.Plugins.Import;
using fluXis.Game.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Plugins;

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
