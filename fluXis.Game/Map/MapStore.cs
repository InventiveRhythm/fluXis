using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Utils;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map;

public class MapStore
{
    private readonly Storage storage;
    private readonly Storage files;
    private readonly FluXisRealm realm;

    public List<RealmMapSet> MapSets { get; } = new();
    public List<RealmMapSet> MapSetsSorted => MapSets.OrderBy(x => x.Metadata.Title).ToList();
    public RealmMapSet CurrentMapSet;

    public Action<RealmMapSet> MapSetAdded;
    public Action<RealmMapSet, RealmMapSet> MapSetUpdated;

    public MapStore(Storage storage, FluXisRealm realm)
    {
        this.storage = storage;
        files = storage.GetStorageForDirectory("files");
        this.realm = realm;

        Logger.Log("Loading maps...");

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

    private void loadMapSets(IEnumerable<RealmMapSet> sets)
    {
        Logger.Log($"Found {sets.Count()} maps");

        foreach (var map in sets)
            AddMapSet(map.Detach());
    }

    public void AddMapSet(RealmMapSet mapSet)
    {
        MapSets.Add(mapSet);
        MapSetAdded?.Invoke(mapSet);
    }

    public void UpdateMapSet(RealmMapSet oldMapSet, RealmMapSet newMapSet)
    {
        MapSets.Remove(oldMapSet);
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

            foreach (var file in mapSetToDelete.Files)
                r.Remove(file);

            r.Remove(mapSetToDelete);

            MapSets.Remove(mapSet);
        });
    }

    public RealmMapSet GetRandom()
    {
        Random rnd = new Random();
        return MapSets[rnd.Next(MapSets.Count)];
    }

    public void Export(RealmMapSet set, LoadingNotification notification)
    {
        try
        {
            string folder = storage.GetFullPath("export");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"{set.Metadata.Title} - {set.Metadata.Artist}.fms");
            if (File.Exists(path)) File.Delete(path);
            ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create);

            foreach (var file in set.Files)
            {
                var entry = archive.CreateEntry(file.Name);
                using var stream = entry.Open();
                using var fileStream = files.GetStream(file.GetPath());
                fileStream.CopyTo(stream);
            }

            archive.Dispose();
            notification.State = LoadingState.Loaded;
            PathUtils.OpenFolder(folder);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Could not export map");
            notification.State = LoadingState.Failed;
        }
    }
}
