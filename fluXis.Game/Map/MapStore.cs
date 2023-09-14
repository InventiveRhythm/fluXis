using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map;

public partial class MapStore : Component
{
    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    private Storage files;
    private MapResourceProvider resources;

    public List<RealmMapSet> MapSets { get; } = new();
    public List<RealmMapSet> MapSetsSorted => MapSets.OrderBy(x => x.Metadata.Title).ToList();
    public RealmMapSet CurrentMapSet;

    public Action<RealmMapSet> MapSetAdded;
    public Action<RealmMapSet, RealmMapSet> MapSetUpdated;

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgroundStore, CroppedBackgroundStore croppedBackgroundStore)
    {
        files = storage.GetStorageForDirectory("maps");

        Logger.Log("Loading maps...");

        resources = new MapResourceProvider
        {
            BackgroundStore = backgroundStore,
            CroppedBackgroundStore = croppedBackgroundStore,
            TrackStore = audio.GetTrackStore(new StorageBackedResourceStore(files))
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

    private void loadMapSets(IQueryable<RealmMapSet> sets)
    {
        Logger.Log($"Found {sets.Count()} maps");

        foreach (var set in sets) AddMapSet(set.Detach());
    }

    public void AddMapSet(RealmMapSet mapSet)
    {
        mapSet.Resources ??= resources;

        MapSets.Add(mapSet);
        MapSetAdded?.Invoke(mapSet);
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
    }

    public RealmMapSet GetRandom()
    {
        Random rnd = new Random();
        return MapSets[rnd.Next(MapSets.Count)];
    }

    public RealmMapSet GetFromGuid(Guid guid) => MapSets.FirstOrDefault(set => set.ID == guid);
    public RealmMapSet GetFromGuid(string guid) => GetFromGuid(Guid.Parse(guid));

    public string Export(RealmMapSet set, LoadingNotification notification, bool openFolder = true)
    {
        try
        {
            string exportFolder = storage.GetFullPath("export");
            if (!Directory.Exists(exportFolder)) Directory.CreateDirectory(exportFolder);

            string path = Path.Combine(exportFolder, $"{set.Metadata.Title} - {set.Metadata.Artist} [{set.Metadata.Mapper}].fms");
            if (File.Exists(path)) File.Delete(path);
            ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create);

            var setFolder = MapFiles.GetFullPath(set.ID.ToString());
            var setFiles = Directory.GetFiles(setFolder);

            int max = setFiles.Length;
            int current = 0;

            var fileNames = new List<string>();

            foreach (var fullFilePath in setFiles)
            {
                var file = Path.GetFileName(fullFilePath);
                Logger.Log($"Exporting {file}");

                var entry = archive.CreateEntry(file);
                using var stream = entry.Open();
                using var fileStream = File.OpenRead(fullFilePath);
                fileStream.CopyTo(stream);
                fileNames.Add(file.Replace(setFolder, ""));
                current++;
                notification.Progress = (float)current / max;
            }

            archive.Dispose();
            notification.State = LoadingState.Loaded;
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
        var req = fluxel.CreateAPIRequest($"/map/hash/{hash}");
        req.Perform();
        var json = req.GetResponseString();
        if (json == null) return null;

        var map = JsonConvert.DeserializeObject<APIResponse<APIMap>>(json);
        return map.Data;
    }

    public RealmMap CreateNew()
    {
        var map = RealmMap.CreateNew();
        map.MapSet.Resources = resources;
        return map;
    }
}
