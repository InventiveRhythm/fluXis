using System;
using System.IO;
using System.IO.Compression;
using fluXis.Game.Import;
using fluXis.Import.osu.Map;
using fluXis.Import.osu.Map.Enums;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Import.osu;

public class OsuImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".osz" };
    public override string Name => "osu!mania";
    public override string Color => "#e7659f";

    public override void Import(string path)
    {
        var notification = new LoadingNotification
        {
            TextLoading = "Importing osu! map...",
            TextSuccess = "Imported osu! map!",
            TextFailure = "Failed to import osu! map!"
        };

        Notifications.AddNotification(notification);

        try
        {
            Logger.Log("Importing osu! map: " + path);

            string folder = Path.GetFileNameWithoutExtension(path);

            ZipArchive osz = ZipFile.OpenRead(path);

            foreach (var entry in osz.Entries)
            {
                if (entry.FullName.EndsWith(".osu"))
                {
                    OsuMap map = parseOsuMap(entry);
                    string json = JsonConvert.SerializeObject(map.ToMapInfo());
                    WriteFile(json, folder + "/" + entry.FullName + ".fsc");

                    notification.TextSuccess = $"Imported osu! map: {map.Artist} - {map.Title}";
                }
                else
                    CopyFile(entry, folder);
            }

            osz.Dispose();

            ZipArchive fms = ZipFile.Open(Path.Combine(Storage.GetFullPath("import"), folder + ".fms"), ZipArchiveMode.Create);

            // add all files from the import folder
            foreach (var file in Directory.GetFiles(Path.Combine(Storage.GetFullPath("import"), folder)))
                fms.CreateEntryFromFile(file, Path.GetFileName(file));

            fms.Dispose();
            Directory.Delete(Path.Combine(Storage.GetFullPath("import"), folder), true);

            var import = new FluXisImport
            {
                MapStatus = ID,
                Notification = notification,
                Realm = Realm,
                MapStore = MapStore,
                Storage = Storage,
                Notifications = Notifications
            };
            import.Import(Path.Combine(Storage.GetFullPath("import"), folder + ".fms"));
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing osu! map");
        }
    }

    private OsuMap parseOsuMap(ZipArchiveEntry entry)
    {
        string fileContent = new StreamReader(entry.Open()).ReadToEnd();
        string[] lines = fileContent.Split(Environment.NewLine);

        OsuParser parser = new();
        OsuFileSection section = OsuFileSection.General;

        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                section = sectionFromString(line);
                continue;
            }

            // ignore comment
            if (line.StartsWith("//")) continue;

            // ignore empty lines
            if (string.IsNullOrEmpty(line)) continue;

            // ignore file version
            if (line.StartsWith("osu file format")) continue;

            parser.AddLine(line, section);
        }

        return parser.Parse();
    }

    private OsuFileSection sectionFromString(string line)
    {
        string section = line.Substring(1, line.Length - 2);

        return section switch
        {
            "General" => OsuFileSection.General,
            "Editor" => OsuFileSection.Editor,
            "Metadata" => OsuFileSection.Metadata,
            "Difficulty" => OsuFileSection.Difficulty,
            "Events" => OsuFileSection.Events,
            "TimingPoints" => OsuFileSection.TimingPoints,
            "Colours" => OsuFileSection.Colours,
            "HitObjects" => OsuFileSection.HitObjects,
            _ => OsuFileSection.General
        };
    }
}
