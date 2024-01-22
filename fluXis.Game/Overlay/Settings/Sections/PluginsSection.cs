using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.Sections.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class PluginsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.PuzzlePiece;
    public override string Title => "Plugins";

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
