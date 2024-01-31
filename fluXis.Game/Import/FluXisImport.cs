using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Import;

public class FluXisImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".fms" };

    /**
     * Used to set the status of the next imported mapset.
     */
    public int MapStatus { get; set; } = -2;

    public LoadingNotificationData Notification { get; set; }

    public override void Import(string path)
    {
        if (Notification == null)
        {
            Notification = new LoadingNotificationData
            {
                TextLoading = "Importing mapset...",
                TextSuccess = "Imported mapset!",
                TextFailure = "Failed to import mapset!"
            };

            Notifications.Add(Notification);
        }

        try
        {
            Logger.Log($"Loading mapset from {path}");

            ZipArchive archive = ZipFile.OpenRead(path);

            List<RealmMap> maps = new();

            RealmMapSet mapSet = new(maps);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string hash = GetHash(entry);

                string filename = entry.FullName;

                if (filename.EndsWith(".fsc"))
                {
                    string json = new StreamReader(entry.Open()).ReadToEnd();
                    MapInfo mapInfo = JsonConvert.DeserializeObject<MapInfo>(json);

                    float length = 0;
                    int keys = 0;
                    float bpmMin = float.MaxValue;
                    float bpmMax = float.MinValue;

                    foreach (var point in mapInfo.TimingPoints)
                    {
                        bpmMin = Math.Min(bpmMin, point.BPM);
                        bpmMax = Math.Max(bpmMax, point.BPM);
                    }

                    foreach (var hitObject in mapInfo.HitObjects)
                    {
                        float time = hitObject.Time;
                        if (hitObject.LongNote) time += hitObject.HoldTime;
                        length = Math.Max(length, time);

                        keys = Math.Max(keys, hitObject.Lane);
                    }

                    RealmMap map = new RealmMap(new RealmMapMetadata
                    {
                        Title = mapInfo.Metadata.Title ?? "Untitled",
                        Artist = mapInfo.Metadata.Artist ?? "Unknown",
                        Mapper = mapInfo.Metadata.Mapper ?? "Unknown",
                        Source = mapInfo.Metadata.Source ?? "",
                        Tags = mapInfo.Metadata.Tags ?? "",
                        Audio = mapInfo.AudioFile,
                        Background = mapInfo.BackgroundFile,
                        PreviewTime = mapInfo.Metadata.PreviewTime
                    })
                    {
                        Difficulty = mapInfo.Metadata.Difficulty ?? "Unknown",
                        MapSet = mapSet,
                        Hash = hash,
                        KeyCount = keys,
                        Rating = 0,
                        Status = MapStatus,
                        FileName = filename
                    };

                    MapEvents events = new MapEvents();
                    var effectFileEntry = archive.Entries.FirstOrDefault(e => e.FullName == mapInfo.EffectFile);

                    if (effectFileEntry != null)
                    {
                        string content = new StreamReader(effectFileEntry.Open()).ReadToEnd();
                        events.Load(content);
                    }

                    map.Filters = MapUtils.GetMapFilters(mapInfo, events);
                    maps.Add(map);

                    if (!string.IsNullOrEmpty(mapInfo.CoverFile))
                        mapSet.Cover = mapInfo.CoverFile;

                    // skip metadata lookup if the map is from a different game
                    if (map.Status >= 100) continue;

                    try
                    {
                        var onlineMap = MapStore.LookUpHash(hash);
                        Logger.Log(JsonConvert.SerializeObject(onlineMap));
                        if (onlineMap == null) continue;

                        map.OnlineID = onlineMap.Id;
                        map.Status = onlineMap.Status;
                        mapSet.OnlineID = onlineMap.SetId;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to look up map online.");
                    }
                }
            }

            if (maps.Count > 0)
            {
                Realm.RunWrite(realm =>
                {
                    realm.Add(mapSet);

                    var fullPath = MapFiles.GetFullPath(mapSet.ID.ToString()) + "/";

                    if (!Directory.Exists(fullPath))
                        Directory.CreateDirectory(fullPath);

                    foreach (var entry in archive.Entries)
                    {
                        var filePath = fullPath + entry.FullName;
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        entry.ExtractToFile(filePath, true);
                    }

                    archive.Dispose();
                    MapStore.AddMapSet(mapSet.Detach());

                    try { File.Delete(path); }
                    catch { Logger.Log($"Failed to delete {path}"); }
                });
            }

            Notification.State = LoadingState.Complete;
        }
        catch (Exception e)
        {
            Notification.State = LoadingState.Failed;
            Logger.Error(e, "Failed to import mapset");
        }
    }
}
