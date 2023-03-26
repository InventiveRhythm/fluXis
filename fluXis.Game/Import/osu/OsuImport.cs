using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Import.FluXis;
using fluXis.Game.Import.osu.Map;
using fluXis.Game.Import.osu.Map.Enums;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Import.osu;

public class OsuImport : MapImporter
{
    public OsuImport(FluXisRealm realm, MapStore mapStore, Storage storage, NotificationOverlay notifications)
        : base(realm, mapStore, storage, notifications)
    {
    }

    public virtual Task Import(string path)
    {
        return new Task(() =>
        {
            try
            {
                Logger.Log("Importing osu! map: " + path);

                string folder = Path.GetFileNameWithoutExtension(path);

                Notifications.Post("Importing osu! map...");

                ZipArchive osz = ZipFile.OpenRead(path);

                foreach (var entry in osz.Entries)
                {
                    if (entry.FullName.EndsWith(".osu"))
                    {
                        string json = JsonConvert.SerializeObject(parseOsuMap(entry).ToMapInfo());
                        WriteFile(json, folder + "/" + entry.FullName + ".fsc");
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

                var import = new FluXisImport(Realm, MapStore, Storage, Notifications) { MapStatus = -4 };
                import.Import(Path.Combine(Storage.GetFullPath("import"), folder + ".fms")).Start();
            }
            catch (Exception e)
            {
                Notifications.PostError("Error while importing osu! map");
                Logger.Error(e, "Error while importing osu! map");
            }
        });
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
