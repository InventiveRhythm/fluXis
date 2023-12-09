using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralFoldersSection : SettingsSubSection
{
    public override string Title => "Folders";
    public override IconUsage Icon => FontAwesome.Solid.Folder;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = "Open fluXis folder",
                ButtonText = "Open",
                Action = () => storage.OpenFileExternally(".")
            },
            new SettingsButton
            {
                Label = "Change folder location",
                Enabled = false,
                ButtonText = "Change"
            },
        });
    }
}
