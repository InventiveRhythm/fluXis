using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Overlay.Settings.Sections.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class UserInterfaceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;
    public override LocalisableString Title => LocalizationStrings.Settings.UI.Title;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new UserInterfaceGeneralSection(),
            Divider,
            new UserInterfaceMainMenuSection(),
            Divider,
            new UserInterfaceSongSelectSection()
        });
    }
}
