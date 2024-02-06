using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Overlay.Settings.Sections.Appearance;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

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
