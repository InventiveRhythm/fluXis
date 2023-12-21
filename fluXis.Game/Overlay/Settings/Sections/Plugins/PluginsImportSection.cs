using System.Linq;
using fluXis.Game.Overlay.Settings.Sections.Plugins.Import;
using fluXis.Game.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Plugins;

public partial class PluginsImportSection : SettingsSubSection
{
    public override string Title => "Import Plugins";
    public override IconUsage Icon => FontAwesome.Solid.Plug;

    [BackgroundDependencyLoader]
    private void load(PluginManager plugins)
    {
        foreach (var plugin in plugins.Plugins.Where(p => p is { Importer: not null }))
            Add(new DrawableImportPlugin { Plugin = plugin });
    }
}
