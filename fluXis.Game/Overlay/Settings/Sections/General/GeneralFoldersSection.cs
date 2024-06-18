using System.IO;
using System.IO.Compression;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.General;

public partial class GeneralFoldersSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Folders;
    public override IconUsage Icon => FontAwesome6.Solid.Folder;

    [Resolved]
    private Storage storage { get; set; }

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
                Label = strings.ExportLogs,
                Description = strings.ExportLogsDescription,
                ButtonText = "Export",
                Action = exportLogs
            },
            new SettingsButton
            {
                Label = strings.FoldersChange,
                Enabled = false,
                ButtonText = "Change"
            },
        });
    }

    private void exportLogs()
    {
        const string file_name = "exported-logs.zip";
        var path = storage.GetFullPath($"export/{file_name}");
        var logs = Logger.Storage;

        Logger.Flush();

        if (File.Exists(path))
            File.Delete(path);

        using var stream = File.Create(path);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        foreach (var file in logs.GetFiles("", "*.log"))
            zip.CreateEntryFromFile(logs.GetFullPath(file), file);

        storage.PresentFileExternally(path);
    }
}
