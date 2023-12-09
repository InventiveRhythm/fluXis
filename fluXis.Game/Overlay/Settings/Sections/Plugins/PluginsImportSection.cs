using fluXis.Game.Import;
using fluXis.Game.Overlay.Settings.Sections.Plugins.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Plugins;

public partial class PluginsImportSection : SettingsSubSection
{
    public override string Title => "Import Plugins";
    public override IconUsage Icon => FontAwesome.Solid.Plug;

    [BackgroundDependencyLoader]
    private void load(ImportManager importManager)
    {
        foreach (var plugin in importManager.Plugins)
            Add(new DrawableImportPlugin { Plugin = plugin });
    }
}
