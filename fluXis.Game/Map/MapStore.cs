using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Import;
using fluXis.Game.Map.Builtin.Roundhouse;
using fluXis.Game.Map.Structures;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Requests.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace fluXis.Game.Map;

public partial class MapStore : Component
{
    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GlobalClock clock { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private bool initialLoad;
    private Storage files;
    private MapResourceProvider resources;

    public Bindable<RealmMap> MapBindable { get; } = new();
    public Bindable<RealmMapSet> MapSetBindable { get; } = new();

    public RealmMap CurrentMap
    {
        get => MapBindable.Value;
        set
        {
            MapBindable.Value = value;
            MapSetBindable.Value = value?.MapSet;
        }
    }

    public RealmMapSet CurrentMapSet
    {
        get => MapSetBindable.Value;
        set
        {
            MapSetBindable.Value = value;
            MapBindable.Value = value?.LowestDifficulty;
        }
    }

    public List<RealmMapSet> MapSets { get; } = new();
    public List<RealmMapSet> MapSetsSorted => MapSets.OrderBy(x => x.Metadata.Title).ToList();

    public Action<RealmMapSet> MapSetAdded;
    public Action<RealmMapSet, RealmMapSet> MapSetUpdated;
    public Action CollectionUpdated;

    public List<APIMapSet> DownloadQueue { get; } = new();
    public Action<APIMapSet> DownloadStarted { get; set; }
    public Action<APIMapSet> DownloadFinished { get; set; }

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgroundStore, CroppedBackgroundStore croppedBackgroundStore)
    {
        initialLoad = true;
        files = storage.GetStorageForDirectory("maps");

        Logger.Log("Loading maps...");

        resources = new MapResourceProvider
        {
            BackgroundStore = backgroundStore,
            CroppedBackgroundStore = croppedBackgroundStore,
            TrackStore = audio.GetTrackStore(new StorageBackedResourceStore(files)),
            SampleStore = audio.GetSampleStore(new StorageBackedResourceStore(files))
        };

        realm.RunWrite(r =>
        {
            var mapSets = r.All<RealmMapSet>();

            // migration stuffs
            foreach (var set in mapSets)
            {
                foreach (var map in set.Maps)
                {
                    if (map.Status == -3)
                    {
                        var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Name == "Quaver");
                        if (info != null) map.Status = info.Id;
                    }

                    if (map.Status == -4)
                    {
                        var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Name == "osu!mania");
                        if (info != null) map.Status = info.Id;
                    }
                }
            }

            loadMapSets(mapSets);
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        MapSetBindable.BindValueChanged(e => Logger.Log($"Changed mapset to {e.NewValue?.Metadata.Title} - {e.NewValue?.Metadata.Artist}", LoggingTarget.Runtime, LogLevel.Debug), true);
    }

    private void loadMapSets(IQueryable<RealmMapSet> sets)
    {
        Logger.Log($"Found {sets.Count()} maps");

        foreach (var set in sets) AddMapSet(set.Detach());

        initialLoad = false;
    }

    public void Select(RealmMap map, bool loop = false, bool preview = true)
    {
        CurrentMap = map;

        if (clock == null)
            return;

        clock.Looping = loop;

        if (loop)
            clock.RestartPoint = preview ? map?.Metadata.PreviewTime ?? 0 : 0;
    }

    public void Present(RealmMapSet map)
    {
        game.ShowMap(map);
    }

    public void AddMapSet(RealmMapSet mapSet, bool notify = true)
    {
        mapSet.Resources ??= resources;

        MapSets.Add(mapSet);

        if (!notify) return;

        MapSetAdded?.Invoke(mapSet);
        if (!initialLoad) CollectionUpdated?.Invoke();
    }

    public void UpdateMapSet(RealmMapSet oldMapSet, RealmMapSet newMapSet)
    {
        MapSets.Remove(oldMapSet);
        newMapSet.Resources = oldMapSet.Resources; // keep the resources
        MapSets.Add(newMapSet);
        MapSetUpdated?.Invoke(oldMapSet, newMapSet);

        if (Equals(CurrentMapSet, oldMapSet))
            CurrentMapSet = newMapSet;
    }

    public void DeleteMapSet(RealmMapSet mapSet)
    {
        if (mapSet.Managed)
        {
            // notifications.Post("Cannot delete a managed mapset!");
            return;
        }

        realm.RunWrite(r =>
        {
            RealmMapSet mapSetToDelete = r.Find<RealmMapSet>(mapSet.ID);
            if (mapSetToDelete == null) return;

            foreach (var map in mapSetToDelete.Maps)
                r.Remove(map);

            r.Remove(mapSetToDelete);

            MapSets.Remove(mapSet);
        });

        CollectionUpdated?.Invoke();
    }

    [CanBeNull]
    public RealmMapSet GetRandom()
    {
        if (MapSets.Count == 0) return null;

        Random rnd = new Random();
        return MapSets[rnd.Next(MapSets.Count)];
    }

    public RealmMapSet QuerySet(Guid id) => realm.Run(r => QuerySetFromRealm(r, id)).Detach();
    public RealmMapSet QuerySetFromRealm(Realm realm, Guid id) => realm.Find<RealmMapSet>(id);

    public RealmMapSet GetFromGuid(Guid guid) => MapSets.FirstOrDefault(set => set.ID == guid);
    public RealmMapSet GetFromGuid(string guid) => GetFromGuid(Guid.Parse(guid));

    public string Export(RealmMapSet set, TaskNotificationData notification, bool openFolder = true)
    {
        try
        {
            string exportFolder = storage.GetFullPath("export");
            if (!Directory.Exists(exportFolder)) Directory.CreateDirectory(exportFolder);

            var fileName = $"{set.Metadata.Title} - {set.Metadata.Artist} [{set.Metadata.Mapper}].fms";
            fileName = PathUtils.RemoveAllInvalidPathCharacters(fileName);

            string path = Path.Combine(exportFolder, fileName);
            if (File.Exists(path)) File.Delete(path);
            ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create);

            var setFolder = MapFiles.GetFullPath(set.ID.ToString());
            var setFiles = Directory.GetFiles(setFolder);

            int max = setFiles.Length;
            int current = 0;

            foreach (var fullFilePath in setFiles)
            {
                var file = Path.GetFileName(fullFilePath);
                Logger.Log($"Exporting {file}");

                var entry = archive.CreateEntry(file);
                using var stream = entry.Open();
                using var fileStream = File.OpenRead(fullFilePath);
                fileStream.CopyTo(stream);
                current++;
                notification.Progress = (float)current / max;
            }

            archive.Dispose();
            notification.State = LoadingState.Complete;
            if (openFolder) PathUtils.OpenFolder(exportFolder);
            return path;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Could not export map");
            notification.State = LoadingState.Failed;
            return "";
        }
    }

    [CanBeNull]
    public APIMap LookUpHash(string hash)
    {
        var req = new MapHashLookupRequest(hash);
        req.Perform(fluxel);
        return req.Response.Status != 200 ? null : req.Response.Data;
    }

    public RealmMap CreateNew()
    {
        var map = RealmMap.CreateNew();
        map.Metadata.Mapper = fluxel.LoggedInUser?.Username ?? "Me";
        map.MapSet.Resources = resources;
        return map;
    }

    public RealmMap CreateNewDifficulty(RealmMapSet set, RealmMap map, string name, MapInfo refInfo = null)
    {
        var id = Guid.NewGuid();
        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsc";

        var info = new MapInfo(new MapMetadata
        {
            Title = map.Metadata.Title,
            Artist = map.Metadata.Artist,
            Mapper = map.Metadata.Mapper,
            Difficulty = name,
            Source = map.Metadata.Source,
            Tags = map.Metadata.Tags,
            PreviewTime = map.Metadata.PreviewTime
        })
        {
            AudioFile = map.Metadata.Audio,
            BackgroundFile = map.Metadata.Background,
            CoverFile = map.MapSet.Cover,
            VideoFile = refInfo?.VideoFile ?? "",
            TimingPoints = refInfo?.TimingPoints.Select(x => new TimingPoint
            {
                BPM = x.BPM,
                Time = x.Time,
                Signature = x.Signature,
                HideLines = x.HideLines
            }).ToList() ?? new List<TimingPoint> { new() { BPM = 120, Time = 0, Signature = 4 } }, // Add default timing point to avoid issues
        };

        var realmMap = new RealmMap
        {
            ID = id,
            Difficulty = name,
            Metadata = new RealmMapMetadata
            {
                Title = map.Metadata.Title,
                Artist = map.Metadata.Artist,
                Mapper = map.Metadata.Mapper,
                Source = map.Metadata.Source,
                Tags = map.Metadata.Tags,
                Background = map.Metadata.Background,
                Audio = map.Metadata.Audio,
                PreviewTime = map.Metadata.PreviewTime,
                ColorHex = map.Metadata.ColorHex
            },
            FileName = fileName,
            OnlineID = 0,
            Hash = MapUtils.GetHash(info.Serialize()),
            Filters = MapUtils.GetMapFilters(info, new MapEvents()),
            KeyCount = map.KeyCount,
            MapSet = set
        };

        save(realmMap, info);
        return addDifficultyToSet(set, realmMap);
    }

    public RealmMap CopyToNewDifficulty(RealmMapSet set, RealmMap map, MapInfo refInfo, string name)
    {
        var id = Guid.NewGuid();
        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsc";
        string effectName = "";

        var refEffect = refInfo.GetMapEvents();
        var refEffectString = refEffect.Save();

        if (!string.IsNullOrEmpty(refEffectString))
        {
            effectName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fse";
            string effectPath = MapFiles.GetFullPath(set.GetPathForFile(effectName));
            File.WriteAllText(effectPath, refEffectString);
        }

        var info = new MapInfo(new MapMetadata
        {
            Title = map.Metadata.Title,
            Artist = map.Metadata.Artist,
            Mapper = map.Metadata.Mapper,
            Difficulty = name,
            Source = map.Metadata.Source,
            Tags = map.Metadata.Tags,
            PreviewTime = map.Metadata.PreviewTime
        })
        {
            AudioFile = map.Metadata.Audio,
            BackgroundFile = map.Metadata.Background,
            CoverFile = map.MapSet.Cover,
            VideoFile = refInfo.VideoFile,
            EffectFile = effectName,
            HitObjects = refInfo.HitObjects.Select(x => x.Copy()).ToList(),
            TimingPoints = refInfo.TimingPoints.Select(x => x.Copy()).ToList(),
            ScrollVelocities = refInfo.ScrollVelocities.Select(x => x.Copy()).ToList()
        };

        var realmMap = new RealmMap
        {
            ID = id,
            Difficulty = name,
            Metadata = new RealmMapMetadata
            {
                Title = map.Metadata.Title,
                Artist = map.Metadata.Artist,
                Mapper = map.Metadata.Mapper,
                Source = map.Metadata.Source,
                Tags = map.Metadata.Tags,
                Background = map.Metadata.Background,
                Audio = map.Metadata.Audio,
                PreviewTime = map.Metadata.PreviewTime,
                ColorHex = map.Metadata.ColorHex
            },
            FileName = fileName,
            Hash = MapUtils.GetHash(info.Serialize()),
            Filters = MapUtils.GetMapFilters(info, refEffect),
            KeyCount = map.KeyCount,
            MapSet = set
        };

        save(realmMap, info);
        return addDifficultyToSet(set, realmMap);
    }

    private RealmMap addDifficultyToSet(RealmMapSet set, RealmMap map)
    {
        return realm.RunWrite(r =>
        {
            var rSet = QuerySetFromRealm(r, set.ID);
            map.MapSet = rSet;
            rSet.Maps.Add(map);

            var detached = rSet.Detach();
            UpdateMapSet(set, detached);
            set = detached;

            return set.Maps.FirstOrDefault(m => m.ID == map.ID);
        });
    }

    private void save(RealmMap map, MapInfo info)
    {
        string path = MapFiles.GetFullPath(map.MapSet.GetPathForFile(map.FileName));
        File.WriteAllText(path, info.Serialize());
    }

    public void DeleteDifficultyFromMapSet(RealmMapSet set, RealmMap map)
    {
        realm.RunWrite(r =>
        {
            var rSet = QuerySetFromRealm(r, set.ID);
            var rMap = rSet.Maps.FirstOrDefault(m => m.ID == map.ID);
            if (rMap == null) return;

            r.Remove(rMap);

            try
            {
                var path = rSet.GetPathForFile(map.FileName);
                var fullPath = MapFiles.GetFullPath(path);
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception e)
            {
                notifications.SendError($"Could not delete difficulty file: {e.Message}");
                Logger.Error(e, "Could not delete difficulty");
            }

            var oldSet = GetFromGuid(set.ID);
            var detached = rSet.Detach();
            UpdateMapSet(oldSet, detached);
        });
    }

    public void StartDownload(APIMapSet mapSet)
    {
        DownloadQueue.Add(mapSet);
        DownloadStarted?.Invoke(mapSet);
    }

    public void FinishDownload(APIMapSet mapSet)
    {
        DownloadQueue.Remove(mapSet);
        DownloadFinished?.Invoke(mapSet);
    }

    public void DownloadMapSet(APIMapSet set)
    {
        if (set == null)
            return;

        if (MapSets.Any(x => x.OnlineID == set.Id))
        {
            notifications.SendText("Mapset already downloaded.");
            return;
        }

        if (DownloadQueue.Any(x => x.Id == set.Id))
            return;

        var notification = new TaskNotificationData
        {
            Text = $"{set.Title} - {set.Artist}",
            TextWorking = "Downloading...",
            TextFinished = "Done! Click to view."
        };

        var req = fluxel.CreateAPIRequest($"/mapset/{set.Id}/download");
        req.DownloadProgress += (current, total) => notification.Progress = (float)current / total;
        req.Started += () => Logger.Log($"Downloading mapset: {set.Title} - {set.Artist}", LoggingTarget.Network);
        req.Failed += exception =>
        {
            Logger.Log($"Failed to download mapset: {exception.Message}", LoggingTarget.Network);
            notification.State = LoadingState.Failed;

            FinishDownload(set);
        };
        req.Finished += () =>
        {
            notification.Progress = 1;
            FinishDownload(set);

            try
            {
                Logger.Log($"Finished downloading mapset: {set.Title} - {set.Artist}", LoggingTarget.Network);
                var data = req.GetResponseData();

                if (data == null)
                {
                    notification.State = LoadingState.Failed;
                    return;
                }

                // write data to file
                var path = storage.GetFullPath($"download/{set.Id}.zip");
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllBytes(path, data);

                // import
                new FluXisImport
                {
                    MapStore = this,
                    Storage = storage,
                    Notifications = notifications,
                    Realm = realm,
                    Notification = notification
                }.Import(path);
            }
            catch (Exception ex)
            {
                notification.State = LoadingState.Failed;
                Logger.Log($"Failed to import mapset: {ex.Message}", LoggingTarget.Network);
            }
        };

        StartDownload(set);
        req.PerformAsync();

        notifications.AddTask(notification);
    }

    public void Remove(RealmMapSet map)
    {
        MapSets.Remove(map);
        CollectionUpdated?.Invoke();
    }

    public static RealmMap CreateDummyMap()
    {
        var set = new RealmMapSet(new List<RealmMap>
        {
            new()
            {
                ID = default,
                Hash = "dummy",
                Metadata = new RealmMapMetadata
                {
                    Title = "please load a map!",
                    Artist = "no maps available!"
                },
                Filters = new RealmMapFilters()
            }
        });

        return set.Maps.FirstOrDefault();
    }

    public RealmMapSet CreateBuiltinMap(BuiltinMap map)
    {
        var set = map switch
        {
            BuiltinMap.Roundhouse => new RoundhouseMapSet(),
            _ => throw new ArgumentOutOfRangeException(nameof(map), map, null)
        };

        set.Resources = new MapResourceProvider { TrackStore = audio.Tracks };
        return set;
    }

    public enum BuiltinMap
    {
        Roundhouse
    }
}
