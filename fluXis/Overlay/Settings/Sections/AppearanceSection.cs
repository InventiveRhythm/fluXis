using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.Appearance;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class AppearanceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;
    public override LocalisableString Title => LocalizationStrings.Settings.Appearance.Title;

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
