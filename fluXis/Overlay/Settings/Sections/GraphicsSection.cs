using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class GraphicsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Display;
    public override LocalisableString Title => LocalizationStrings.Settings.Graphics.Title;

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
