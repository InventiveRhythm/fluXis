using fluXis.Game.Overlay.Settings.Sections.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GraphicsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Desktop;
    public override string Title => "Graphics";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new GraphicsLayoutSection(),
            Divider,
            new GraphicsRenderingSection()
        });
    }
}
