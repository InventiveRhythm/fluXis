using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Utils;
using fluXis.Shared.Utils;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Import;

public class FluXisImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".fms" };

    /**
     * Used to set the status of the next imported mapset.
     */
    public int MapStatus { get; set; } = -2;

    public TaskNotificationData Notification { get; set; }

    public override void Import(string path)
    {
        if (!File.Exists(path))
            return;

        if (Notification == null)
        {
            Notification = new TaskNotificationData
            {
                Text = "Importing Mapset...",
                TextWorking = "Processing...",
                TextFinished = "Done! Click to view."
            };

            Notifications.AddTask(Notification);
        }

        try
        {
            Logger.Log($"Loading mapset from {path}");

            var set = getRealmMaps(path);

            var existing = Realm.Run(r =>
            {
                var sets = r.All<RealmMapSet>();
                return sets.FirstOrDefault(x => x.OnlineID == set.OnlineID)?.Detach();
            });

            if (existing != null && existing.OnlineID > 0)
            {
                var updatedPath = MapFiles.GetFullPath(set.ID.ToString()) + "/";
                var originalPath = MapFiles.GetFullPath(existing.ID.ToString()) + "/";
                var files = Directory.GetFiles(updatedPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    Logger.Log($"Moving {file}");
                    var f = new FileInfo(file);
                    f.MoveTo($"{originalPath}/{f.Name}", true);
                }

                Directory.Delete(updatedPath);

                importAsUpdate(set, existing);
            }
            else
            {
                if (set.Maps.Count > 0)
                {
                    Realm.RunWrite(realm =>
                    {
                        realm.Add(set);

                        set = set.Detach();
                        MapStore.AddMapSet(set);
                    });
                }
            }

            try { File.Delete(path); }
            catch { Logger.Log($"Failed to delete {path}"); }

            Notification.ClickAction = () => MapStore.Present(set);
            Notification.State = LoadingState.Complete;
        }
        catch (Exception e)
        {
            Notification.State = LoadingState.Failed;
            Logger.Error(e, "Failed to import mapset");
        }
    }

    public void ImportAsUpdate(string path, RealmMapSet original)
    {
        var updated = getRealmMaps(path, original.ID);
        importAsUpdate(updated, original);
    }

    private void importAsUpdate(RealmMapSet updated, RealmMapSet original)
    {
        Realm.RunWrite(r =>
        {
            // replace with realm version
            original = r.Find<RealmMapSet>(original.ID);
            updated.ID = original.ID;

            foreach (var oMap in original.Maps)
            {
                var uMap = updated.Maps.FirstOrDefault(x => x.FileName == oMap.FileName);

                // map got deleted in the new version
                if (uMap == null)
                {
                    original.Maps.Remove(oMap);
                    r.Remove(oMap);
                    continue;
                }

                uMap.ID = oMap.ID;
            }

            foreach (var uMap in updated.Maps)
            {
                var oMap = original.Maps.FirstOrDefault(x => x.FileName == uMap.FileName);

                if (uMap == null)
                {
                    original.Maps.Remove(oMap);
                    r.Remove(oMap);
                }
            }

            updated.CopyChanges(original);

            original = original.Detach();
        });

        MapStore.UpdateMapSet(MapStore.GetFromGuid(original.ID), original);
    }

    private RealmMapSet getRealmMaps(string path, Guid? id = null)
    {
        using var archive = ZipFile.OpenRead(path);
        var maps = new List<RealmMap>();

        Logger.Log(id.ToString());

        var mapSet = new RealmMapSet(maps)
        {
            ID = id ?? Guid.NewGuid()
        };
        Logger.Log(mapSet.ID.ToString());

        MapStore.AssignResources(mapSet);

        var fullPath = MapFiles.GetFullPath(mapSet.ID.ToString()) + "/";

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        foreach (var entry in archive.Entries)
        {
            var filePath = fullPath + entry.FullName;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            entry.ExtractToFile(filePath, true);
        }

        var fileCount = archive.Entries.Count;
        var idx = 0;

        foreach (var entry in archive.Entries)
        {
            var hash = GetHash(entry);
            var filename = entry.FullName;

            if (filename.EndsWith(".fsc"))
            {
                var json = new StreamReader(entry.Open()).ReadToEnd();
                var mapInfo = json.Deserialize<MapInfo>();

                var length = 0f;
                var keys = 0;
                var bpmMin = float.MaxValue;
                var bpmMax = float.MinValue;

                foreach (var point in mapInfo.TimingPoints)
                {
                    bpmMin = Math.Min(bpmMin, point.BPM);
                    bpmMax = Math.Max(bpmMax, point.BPM);
                }

                foreach (var hitObject in mapInfo.HitObjects)
                {
                    var time = hitObject.Time;

                    if (hitObject.LongNote)
                        time += hitObject.HoldTime;

                    length = (float)Math.Max(length, time);
                    keys = Math.Max(keys, hitObject.Lane);
                }

                var map = new RealmMap
                {
                    Metadata = new RealmMapMetadata
                    {
                        Title = mapInfo.Metadata.Title ?? "",
                        Artist = mapInfo.Metadata.Artist ?? "",
                        Mapper = mapInfo.Metadata.Mapper ?? "",
                        Source = mapInfo.Metadata.AudioSource ?? "",
                        Tags = mapInfo.Metadata.Tags ?? "",
                        Audio = mapInfo.AudioFile,
                        Background = mapInfo.BackgroundFile,
                        PreviewTime = mapInfo.Metadata.PreviewTime,
                        ColorHex = string.IsNullOrEmpty(mapInfo.Colors.AccentHex) ? "" : mapInfo.Colors.AccentHex,
                    },
                    Difficulty = mapInfo.Metadata.Difficulty ?? "",
                    MapSet = mapSet,
                    Hash = hash,
                    KeyCount = keys,
                    StatusInt = MapStatus,
                    FileName = filename
                };

                mapInfo.Map = map;

                if (string.IsNullOrEmpty(map.Metadata.ColorHex))
                {
                    var background = map.GetBackgroundStream();

                    if (background != null)
                    {
                        var color = ImageUtils.GetAverageColour(background);

                        if (color != Colour4.Transparent)
                            map.Metadata.Color = color;
                    }
                    else
                        Logger.Log("Failed to load background for color extraction");
                }

                var events = mapInfo.GetMapEvents();

                foreach (var switchEvent in events.LaneSwitchEvents)
                    map.KeyCount = Math.Max(map.KeyCount, switchEvent.Count);

                map.Filters = MapUtils.GetMapFilters(mapInfo, events);
                maps.Add(map);

                if (!string.IsNullOrEmpty(mapInfo.CoverFile))
                    mapSet.Cover = mapInfo.CoverFile;

                // skip metadata lookup if the map is from a different game
                if (map.StatusInt >= 100)
                    continue;

                try
                {
                    var lookup = MapStore.LookUpHash(hash);
                    if (lookup == null) continue;

                    map.OnlineID = (int)lookup.ID;
                    map.StatusInt = lookup.Status;
                    mapSet.OnlineID = (int)lookup.SetID;
                    mapSet.DateSubmitted = TimeUtils.GetFromSeconds(lookup.DateSubmitted);

                    if (lookup.DateRanked != null)
                        mapSet.DateRanked = TimeUtils.GetFromSeconds((long)lookup.DateRanked);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to look up map online.");
                }
            }

            idx++;
            Notification.Progress = (float)idx / fileCount;
        }

        return mapSet;
    }
}
