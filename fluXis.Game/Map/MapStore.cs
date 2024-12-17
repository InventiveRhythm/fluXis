using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Import;
using fluXis.Game.Map.Builtin.Christmashouse;
using fluXis.Game.Map.Builtin.Roundhouse;
using fluXis.Game.Map.Builtin.Spoophouse;
using fluXis.Game.Map.Structures;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Requests.Maps;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Storyboards;
using fluXis.Game.Utils;
using fluXis.Game.Utils.Extensions;
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
    private IAPIClient api { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GlobalClock clock { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private Storage files;
    private MapSetResources resources;

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

    #region Loading

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgroundStore, CroppedBackgroundStore croppedBackgroundStore)
    {
        files = storage.GetStorageForDirectory("maps");

        Logger.Log("Loading mapsets...");

        resources = new MapSetResources
        {
            BackgroundStore = backgroundStore,
            CroppedBackgroundStore = croppedBackgroundStore,
            TrackStore = audio.GetTrackStore(new StorageBackedResourceStore(files)),
            SampleStore = audio.GetSampleStore(new StorageBackedResourceStore(files))
        };

        realm.RunWrite(r =>
        {
            var sets = r.All<RealmMapSet>();

            fixSetData(sets);

            foreach (var set in sets)
                AddMapSet(set.Detach());

            Logger.Log($"Loaded {MapSets.Count} mapsets.");
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        MapSetBindable.BindValueChanged(e => Logger.Log($"Changed selected mapset to {e.NewValue?.Metadata.Title} - {e.NewValue?.Metadata.Artist}", LoggingTarget.Runtime, LogLevel.Debug), true);
    }

    /// <summary>
    /// tries to fix potentially broken
    /// database items with null data
    /// </summary>
    /// <param name="sets">the sets to fix</param>
    private void fixSetData(IQueryable<RealmMapSet> sets)
    {
        foreach (var set in sets)
        {
            foreach (var map in set.Maps)
            {
                map.Filters ??= new RealmMapFilters();
                map.Metadata ??= new RealmMapMetadata();
            }
        }
    }

    internal void AssignResources(RealmMapSet mapSet) => mapSet.Resources ??= resources;

    #endregion

    #region Collection Management

    public List<RealmMapSet> MapSets { get; } = new();

    public Action<RealmMapSet> MapSetAdded;
    public Action<RealmMapSet, RealmMapSet> MapSetUpdated;
    public Action CollectionUpdated;

    public void AddMapSet(RealmMapSet set, bool notify = true)
    {
        AssignResources(set);

        MapSets.Add(set);

        if (!notify) return;

        MapSetAdded?.Invoke(set);
        if (!IsLoaded) CollectionUpdated?.Invoke();
    }

    public void UpdateMapSet(RealmMapSet oldMapSet, RealmMapSet newMapSet)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            MapSets.Remove(oldMapSet);
            newMapSet.Resources = oldMapSet.Resources; // keep the resources
            MapSets.Add(newMapSet);
            MapSetUpdated?.Invoke(oldMapSet, newMapSet);

            if (Equals(CurrentMapSet, oldMapSet))
                CurrentMapSet = newMapSet;
        });
    }

    /// <summary>
    /// Completely deletes a mapset from the database and notifies of the removal.
    /// </summary>
    /// <param name="set">The set to delete.</param>
    public void DeleteMapSet(RealmMapSet set)
    {
        if (set.AutoImported)
            return;

        realm.RunWrite(r =>
        {
            RealmMapSet mapSetToDelete = r.Find<RealmMapSet>(set.ID);
            if (mapSetToDelete == null) return;

            foreach (var map in mapSetToDelete.Maps)
            {
                r.Remove(map.Metadata);
                r.Remove(map.Filters);
                r.Remove(map);
            }

            r.Remove(mapSetToDelete);

            MapSets.Remove(set);
        });

        CollectionUpdated?.Invoke();
    }

    /// <summary>
    /// Removed a mapset from the list in memory. Notifies of the removal.
    /// </summary>
    /// <param name="set"></param>
    public void RemoveMapSet(RealmMapSet set)
    {
        MapSets.Remove(set);
        CollectionUpdated?.Invoke();
    }

    #endregion

    #region Queries

    public RealmMapSet GetFromGuid(Guid guid) => MapSets.FirstOrDefault(set => set.ID == guid);
    public RealmMapSet GetFromGuid(string guid) => GetFromGuid(Guid.Parse(guid));

    [CanBeNull]
    public RealmMap GetMapFromGuid(Guid guid)
        => MapSets.FirstOrDefault(set => set.Maps.Any(m => m.ID == guid))?
            .Maps.FirstOrDefault(m => m.ID == guid);

    public RealmMapSet QuerySet(Guid id) => realm.Run(r => QuerySetFromRealm(r, id)).Detach();
    public RealmMapSet QuerySetFromRealm(Realm realm, Guid id) => realm.Find<RealmMapSet>(id);

    [CanBeNull]
    public RealmMapSet GetRandom()
    {
        if (MapSets.Count == 0) return null;

        Random rnd = new Random();
        return MapSets[rnd.Next(MapSets.Count)];
    }

    #endregion

    #region Downloads/Lookup

    public List<DownloadStatus> DownloadQueue { get; } = new();
    public Action<DownloadStatus> DownloadStarted { get; set; }
    public Action<DownloadStatus> DownloadFinished { get; set; }

    private void startDownload(DownloadStatus status)
    {
        DownloadQueue.Add(status);
        DownloadStarted?.Invoke(status);
    }

    private void finishDownload(DownloadStatus status)
    {
        DownloadQueue.Remove(status);
        DownloadFinished?.Invoke(status);
    }

    public void DownloadMapSet(long id)
    {
        if (DownloadQueue.Any(x => x.OnlineID == id))
            return;

        var req = new MapSetRequest(id);
        req.Failure += ex => notifications.SendError("Failed to download mapset", ex.Message);
        api.PerformRequest(req);

        if (!req.IsSuccessful)
            return;

        DownloadMapSet(req.Response!.Data);
    }

    public void DownloadMapSet(APIMapSet set)
    {
        if (set == null)
            return;

        if (MapSets.Any(x => x.OnlineID == set.ID))
        {
            notifications.SendText("Mapset already downloaded.");
            return;
        }

        if (DownloadQueue.Any(x => x.OnlineID == set.ID))
            return;

        var notification = new TaskNotificationData
        {
            Text = $"{set.Title} - {set.Artist}",
            TextWorking = "Downloading...",
            TextFinished = "Done! Click to view."
        };

        Logger.Log($"Downloading mapset: {set.Title} - {set.Artist}", LoggingTarget.Network);

        var status = new DownloadStatus(set.ID);

        var req = new MapSetDownloadRequest(set.ID);
        req.Progress += (current, total) => notification.Progress = status.Progress = (float)current / total;
        req.Failure += exception =>
        {
            Logger.Log($"Failed to download mapset: {exception.Message}", LoggingTarget.Network);
            notification.State = LoadingState.Failed;

            finishDownload(status);
        };
        req.Success += () =>
        {
            notification.Progress = status.Progress = 1;
            status.State = DownloadState.Importing;

            Task.Run(() =>
            {
                try
                {
                    Logger.Log($"Finished downloading mapset: {set.Title} - {set.Artist}", LoggingTarget.Network);
                    var data = req.ResponseStream;

                    if (data == null)
                    {
                        notification.State = LoadingState.Failed;
                        status.State = DownloadState.Failed;
                        return;
                    }

                    // write data to file
                    var path = storage.GetFullPath($"download/{set.ID}.zip");
                    var dir = Path.GetDirectoryName(path);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(path))
                        File.Delete(path);

                    File.WriteAllBytes(path, data.ToArray());

                    // import
                    new FluXisImport
                    {
                        MapStore = this,
                        Storage = storage,
                        Notifications = notifications,
                        Realm = realm,
                        Notification = notification,
                        OnComplete = s => status.State = s ? DownloadState.Finished : DownloadState.Failed,
                        OnProgress = p => status.Progress = p
                    }.Import(path);
                }
                catch (Exception ex)
                {
                    notification.State = LoadingState.Failed;
                    status.State = DownloadState.Failed;
                    Logger.Log($"Failed to import mapset: {ex.Message}", LoggingTarget.Network);
                }
                finally
                {
                    finishDownload(status);
                }
            });
        };

        startDownload(status);
        api.PerformRequestAsync(req);

        notifications.AddTask(notification);
    }

    public void DownloadMapSetUpdate(RealmMapSet set)
    {
        if (set is not { OnlineID: > 0 })
            return;

        if (DownloadQueue.Any(x => x.OnlineID == set.OnlineID))
            return;

        var notification = new TaskNotificationData
        {
            Text = $"{set.Metadata.Title} - {set.Metadata.Artist}",
            TextWorking = "Updating...",
            TextFinished = "Done! Click to view."
        };

        Logger.Log($"Updating mapset: {set.Metadata.Title} - {set.Metadata.Artist}", LoggingTarget.Network);

        var status = new DownloadStatus(set.OnlineID);

        var req = new MapSetDownloadRequest(set.OnlineID);
        req.Progress += (current, total) => notification.Progress = (float)current / total;
        req.Failure += exception =>
        {
            Logger.Log($"Failed to download mapset: {exception.Message}", LoggingTarget.Network);
            notification.State = LoadingState.Failed;
            status.State = DownloadState.Failed;
            finishDownload(status);
        };
        req.Success += () =>
        {
            notification.Progress = status.Progress = 1;
            status.State = DownloadState.Importing;

            Task.Run(() =>
            {
                try
                {
                    Logger.Log($"Finished downloading mapset: {set.Metadata.Title} - {set.Metadata.Artist}", LoggingTarget.Network);
                    var data = req.ResponseStream;

                    if (data == null)
                    {
                        notification.State = LoadingState.Failed;
                        status.State = DownloadState.Failed;
                        return;
                    }

                    // write data to file
                    var path = storage.GetFullPath($"download/{set.ID}.zip");
                    var dir = Path.GetDirectoryName(path);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(path))
                        File.Delete(path);

                    File.WriteAllBytes(path, data.ToArray());

                    // import
                    new FluXisImport
                    {
                        MapStore = this,
                        Storage = storage,
                        Notifications = notifications,
                        Realm = realm,
                        Notification = notification
                    }.ImportAsUpdate(path, set);
                    notification.State = LoadingState.Complete;
                }
                catch (Exception ex)
                {
                    notification.State = LoadingState.Failed;
                    status.State = DownloadState.Failed;
                    Logger.Log($"Failed to update mapset: {ex.Message}", LoggingTarget.Network);
                }
                finally
                {
                    finishDownload(status);
                }
            });
        };

        startDownload(status);
        api.PerformRequestAsync(req);
        notifications.AddTask(notification);
    }

    public class DownloadStatus
    {
        public long OnlineID { get; }

        public DownloadState State
        {
            get => state;
            set
            {
                state = value;
                StateChanged?.Invoke(state);
            }
        }

        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnProgress?.Invoke(progress);
            }
        }

        public event Action<DownloadState> StateChanged;
        public event Action<float> OnProgress;

        private DownloadState state = DownloadState.Downloading;
        private float progress;

        public DownloadStatus(long onlineID)
        {
            OnlineID = onlineID;
        }
    }

    public enum DownloadState
    {
        Downloading,
        Importing,
        Finished,
        Failed
    }

    [CanBeNull]
    public APIMapLookup LookUpHash(string hash)
    {
        var req = new MapLookupRequest
        {
            Hash = hash
        };

        api.PerformRequest(req);
        return req.IsSuccessful ? req.Response!.Data : null;
    }

    #endregion

    #region Editing

    public RealmMap CreateNew()
    {
        var map = RealmMap.CreateNew();
        map.Metadata.Mapper = api.User.Value?.Username ?? "Me";
        map.MapSet.Resources = resources;
        return realm.RunWrite(r =>
        {
            var set = r.Add(map.MapSet);
            set = set.Detach();
            AddMapSet(set);
            return set.Maps.FirstOrDefault(m => m.ID == map.ID);
        });
    }

    public void Save(RealmMap map, MapInfo info, MapEvents events, Storyboard storyboard, bool setStatus)
    {
        if (map == null || info == null)
            throw new ArgumentNullException();

        var set = map.MapSet;

        if (setStatus)
        {
            map.LastLocalUpdate = DateTimeOffset.Now;
            map.Status = MapStatus.Local;
            map.ResetOnlineInfo();
        }

        var directory = MapFiles.GetFullPath(set.GetPathForFile(""));

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (events is { Empty: false })
        {
            var effectFilename = string.IsNullOrWhiteSpace(info.EffectFile) ? $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.ffx" : info.EffectFile;
            File.WriteAllText(MapFiles.GetFullPath(set.GetPathForFile(effectFilename)), events.Save());
            info.EffectFile = effectFilename;
        }
        else
            info.EffectFile = "";

        if (storyboard is { Empty: false })
        {
            var filename = string.IsNullOrWhiteSpace(info.StoryboardFile) ? $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsb" : info.StoryboardFile;
            File.WriteAllText(MapFiles.GetFullPath(set.GetPathForFile(filename)), storyboard.Serialize());
            info.StoryboardFile = filename;
        }
        else
            info.StoryboardFile = "";

        realm.RunWrite(r =>
        {
            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(info.Serialize()));
            stream.Seek(0, SeekOrigin.Begin);

            var filename = string.IsNullOrWhiteSpace(map.FileName) ? $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsc" : map.FileName;
            File.WriteAllBytes(MapFiles.GetFullPath(set.GetPathForFile(filename)), stream.ToArray());

            var hash = MapUtils.GetHash(stream);
            map.Hash = hash;
            map.FileName = filename;

            map.Filters ??= new RealmMapFilters();
            map.Filters.UpdateFilters(info, events);

            var existing = r.Find<RealmMap>(map.ID)!;
            set.CopyChanges(existing.MapSet);
        });
    }

    public RealmMap CreateNewDifficulty(RealmMapSet set, RealmMap map, MapInfo refInfo, CreateNewMapParameters param)
    {
        var id = Guid.NewGuid();
        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsc";
        var effectName = "";
        var refEffect = refInfo.GetMapEvents();

        if (param.LinkEffects)
            effectName = refInfo.EffectFile;
        else if (!refEffect.Empty)
        {
            effectName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.ffx";
            string effectPath = MapFiles.GetFullPath(set.GetPathForFile(effectName));
            File.WriteAllText(effectPath, refEffect.Save());
        }

        var info = refInfo.JsonCopy();
        info.Metadata.Difficulty = param.DifficultyName;
        info.EffectFile = effectName;

        if (!param.CopyNotes)
            info.HitObjects = new List<HitObject>();

        var realmMap = new RealmMap
        {
            ID = id,
            Difficulty = param.DifficultyName,
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
            MapSet = set,
            AccuracyDifficulty = map.AccuracyDifficulty,
            HealthDifficulty = map.HealthDifficulty
        };

        string path = MapFiles.GetFullPath(map.MapSet.GetPathForFile(map.FileName));
        File.WriteAllText(path, info.Serialize());
        return addDifficultyToSet(set, realmMap);
    }

    public void DeleteDifficulty(RealmMapSet set, RealmMap map)
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

    #endregion

    #region Builtin/Dummy

    public RealmMapSet CreateBuiltinMap(BuiltinMap map)
    {
        RealmMapSet set = map switch
        {
            BuiltinMap.Roundhouse => new RoundhouseMapSet(),
            BuiltinMap.Spoophouse => new SpoophouseMapSet(),
            BuiltinMap.Christmashouse => new ChristmashouseMapSet(),
            _ => throw new ArgumentOutOfRangeException(nameof(map), map, null)
        };

        set.Resources = new MapSetResources { TrackStore = audio.Tracks };
        return set;
    }

    public enum BuiltinMap
    {
        Roundhouse,
        Spoophouse,
        Christmashouse,
    }

    public static RealmMap CreateDummyMap()
    {
        var set = new RealmMapSet(new List<RealmMap>
        {
            new()
            {
                ID = Guid.Empty,
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

    #endregion

    #region Misc

    public void Select(RealmMap map, bool loop = false, bool preview = true)
    {
        CurrentMap = map;

        if (clock == null)
            return;

        clock.Looping = loop;

        if (loop)
            clock.RestartPoint = preview ? Math.Max(map.Metadata.PreviewTime, 0) : 0;
    }

    public void Present(RealmMapSet map)
    {
        game.ShowMap(map);
    }

    public string Export(RealmMapSet set, TaskNotificationData notification, bool openFolder = true)
    {
        try
        {
            var fileName = $"{set.Metadata.Title} - {set.Metadata.Artist} [{set.Metadata.Mapper}].fms";
            fileName = PathUtils.RemoveAllInvalidPathCharacters(fileName);

            string path = game.ExportStorage.GetFullPath(fileName);
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
            if (openFolder) game.ExportStorage.PresentFileExternally(fileName);
            return path;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Could not export map");
            notification.State = LoadingState.Failed;
            return "";
        }
    }

    #endregion
}
