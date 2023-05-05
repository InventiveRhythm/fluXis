using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Import.FluXis;
using fluXis.Game.Import.Quaver.Map;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;
using YamlDotNet.Serialization;

namespace fluXis.Game.Import.Quaver;

public class QuaverImport : MapImporter
{
    public QuaverImport(FluXisRealm realm, MapStore mapStore, Storage storage, NotificationOverlay notifications)
        : base(realm, mapStore, storage, notifications)
    {
    }

    public virtual Task Import(string path)
    {
        return new Task(() =>
        {
            var notification = new LoadingNotification
            {
                TextLoading = "Importing Quaver map...",
                TextSuccess = "Imported Quaver map!",
                TextFailure = "Failed to import Quaver map!"
            };

            Notifications.AddNotification(notification);

            try
            {
                Logger.Log("Importing Quaver map: " + path);
                string fileName = Path.GetFileNameWithoutExtension(path);

                ZipArchive qp = ZipFile.OpenRead(path);

                List<MapInfo> mapInfos = new();

                foreach (var entry in qp.Entries)
                {
                    if (entry.FullName.EndsWith(".qua"))
                    {
                        QuaverMap quaverMap = parseQuaverMap(entry);
                        MapInfo map = quaverMap.ToMapInfo();
                        mapInfos.Add(map);

                        notification.TextSuccess = $"Imported Quaver map: {map.Metadata.Artist} - {map.Metadata.Title}";

                        string effect = quaverMap.GetEffects();
                        Logger.Log(effect);

                        if (effect != "")
                        {
                            string name = entry.FullName.ToLower() + ".ffx";
                            string dest = Path.Combine(Storage.GetFullPath("import"), fileName, name);
                            Directory.CreateDirectory(Path.GetDirectoryName(dest));
                            File.WriteAllText(dest, effect);

                            map.EffectFile = name;
                        }

                        string json = JsonConvert.SerializeObject(map);
                        string destPath = Path.Combine(Storage.GetFullPath("import"), fileName, entry.FullName + ".fsc");
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        File.WriteAllText(destPath, json);
                    }
                    else CopyFile(entry, fileName);
                }

                qp.Dispose();

                ZipArchive fms = ZipFile.Open(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms"), ZipArchiveMode.Create);

                // add all files from the import folder
                foreach (var file in Directory.GetFiles(Path.Combine(Storage.GetFullPath("import"), fileName)))
                    fms.CreateEntryFromFile(file, Path.GetFileName(file));

                fms.Dispose();
                Directory.Delete(Path.Combine(Storage.GetFullPath("import"), fileName), true);

                var import = new FluXisImport(Realm, MapStore, Storage, Notifications)
                {
                    MapStatus = -3,
                    Notification = notification
                };
                import.Import(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms")).Start();
            }
            catch (Exception e)
            {
                notification.State = LoadingState.Failed;
                Logger.Error(e, "Error while importing Quaver map");
            }
        });
    }

    private QuaverMap parseQuaverMap(ZipArchiveEntry entry)
    {
        string yaml = new StreamReader(entry.Open()).ReadToEnd();

        var builder = new DeserializerBuilder();
        builder.IgnoreUnmatchedProperties();
        var deserializer = builder.Build();

        QuaverMap map = deserializer.Deserialize<QuaverMap>(yaml);
        return map;
    }
}
