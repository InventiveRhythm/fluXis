using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Import;

public class FluXisImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".fms" };

    /**
     * Used to set the status of the next imported mapset.
     */
    public int MapStatus { get; set; } = -2;

    public Action<float> OnProgress { get; set; }
    public Action<bool> OnComplete { get; set; }

    public TaskNotificationData Notification { get; set; }

    public override void Import(string path)
    {
        if (!File.Exists(path))
        {
            OnComplete?.Invoke(false);
            return;
        }

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
            OnComplete?.Invoke(true);
        }
        catch (Exception e)
        {
            Notification.State = LoadingState.Failed;
            Logger.Error(e, "Failed to import mapset");
            OnComplete?.Invoke(false);
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

        MapStore.UpdateMapSet(original, original);
    }

    private RealmMapSet getRealmMaps(string path, Guid? id = null)
    {
        using var archive = ZipFile.OpenRead(path);
        var maps = new List<RealmMap>();

        var mapSet = new RealmMapSet(maps)
        {
            ID = id ?? Guid.NewGuid()
        };

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
                var keys = mapInfo.SinglePlayerKeyCount;
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
                }

                var map = new RealmMap
                {
                    Metadata = new RealmMapMetadata
                    {
                        Title = mapInfo.Metadata.Title ?? "",
                        TitleRomanized = mapInfo.Metadata.TitleRomanized ?? mapInfo.Metadata.Title ?? "",
                        Artist = mapInfo.Metadata.Artist ?? "",
                        ArtistRomanized = mapInfo.Metadata.ArtistRomanized ?? mapInfo.Metadata.Artist ?? "",
                        Mapper = mapInfo.Metadata.Mapper ?? "",
                        Source = mapInfo.Metadata.AudioSource ?? "",
                        Tags = mapInfo.Metadata.Tags ?? "",
                        Audio = mapInfo.AudioFile,
                        Background = mapInfo.BackgroundFile,
                        PreviewTime = mapInfo.Metadata.PreviewTime,
                        ColorHex = string.IsNullOrEmpty(mapInfo.Colors.AccentHex) ? "" : mapInfo.Colors.AccentHex,
                    },
                    Difficulty = mapInfo.Metadata.Difficulty ?? "",
                    AccuracyDifficulty = mapInfo.AccuracyDifficulty,
                    HealthDifficulty = mapInfo.HealthDifficulty,
                    MapSet = mapSet,
                    Hash = hash,
                    KeyCount = keys,
                    StatusInt = MapStatus,
                    FileName = filename
                };

                mapInfo.RealmEntry = map;

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

                    map.OnlineID = lookup.ID;
                    map.StatusInt = lookup.Status;
                    map.Rating = (float)lookup.Rating;
                    mapSet.OnlineID = lookup.SetID;
                    mapSet.DateSubmitted = TimeUtils.GetFromSeconds(lookup.DateSubmitted);
                    map.LastOnlineUpdate = TimeUtils.GetFromSeconds(lookup.LastUpdated);

                    if (lookup.DateRanked != null)
                        mapSet.DateRanked = TimeUtils.GetFromSeconds(lookup.DateRanked.Value);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to look up map online.");
                }
            }

            idx++;

            var prog = (float)idx / fileCount;
            Notification.Progress = prog;
            OnProgress?.Invoke(prog);
        }

        return mapSet;
    }
}
