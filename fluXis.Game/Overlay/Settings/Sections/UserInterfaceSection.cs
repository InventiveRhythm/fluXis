using fluXis.Game.Overlay.Settings.Sections.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class UserInterfaceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.LayerGroup;
    public override string Title => "User Interface";

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
