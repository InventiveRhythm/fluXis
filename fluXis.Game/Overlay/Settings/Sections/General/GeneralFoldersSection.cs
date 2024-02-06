using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralFoldersSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Folders;
    public override IconUsage Icon => FontAwesome6.Solid.Folder;

    private SettingsGeneralStrings strings => LocalizationStrings.Settings.General;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = strings.FoldersOpen,
                ButtonText = "Open",
                Action = () => storage.OpenFileExternally(".")
            },
            new SettingsButton
            {
                Label = strings.FoldersChange,
                Enabled = false,
                ButtonText = "Change"
            },
        });
    }
}
