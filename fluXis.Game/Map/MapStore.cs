using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map;

public class MapStore
{
    private readonly Storage storage;
    private readonly FluXisRealm realm;

    public List<RealmMapSet> MapSets { get; } = new();
    public RealmMapSet CurrentMapSet;

    public MapStore(Storage storage, FluXisRealm realm)
    {
        this.storage = storage.GetStorageForDirectory("files");
        this.realm = realm;

        Logger.Log("Loading maps...");

        realm.Run(r => loadMapSets(r.All<RealmMapSet>()));
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
        sortMapSets();
    }

    public void DeleteMapSet(RealmMapSet mapSet)
    {
        realm.RunWrite(r =>
        {
            RealmMapSet mapSetToDelete = r.Find<RealmMapSet>(mapSet.ID);
            if (mapSetToDelete == null) return;

            foreach (var map in mapSetToDelete.Maps)
                r.Remove(map);

            foreach (var file in mapSetToDelete.Files)
            {
                try
                {
                    storage.Delete(file.GetPath());
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Could not delete file {file.GetPath()}");
                }

                r.Remove(file);
            }

            r.Remove(mapSetToDelete);

            MapSets.Remove(mapSet);
        });
    }

    private void sortMapSets()
    {
        MapSets.Sort((x, y) => string.Compare(x.Metadata.Title, y.Metadata.Title, StringComparison.Ordinal));
    }

    public RealmMapSet GetRandom()
    {
        Random rnd = new Random();
        return MapSets[rnd.Next(MapSets.Count)];
    }
}
