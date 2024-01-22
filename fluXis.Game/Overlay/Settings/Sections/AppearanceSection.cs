using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.Sections.Appearance;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class AppearanceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;
    public override string Title => "Appearance";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new AppearanceSkinSection(),
            Divider,
            new AppearanceLayoutSection()
        });
    }
}
