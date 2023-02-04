using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map
{
    public class MapStore
    {
        private static readonly List<RealmMapSet> sets = new();
        private static Storage storage;

        public List<RealmMapSet> MapSets => sets;

        public RealmMapSet CurrentMapSet;

        private FluXisRealm realm;

        public MapStore(Storage storage, FluXisRealm realm)
        {
            MapStore.storage = storage;
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
            sets.Add(mapSet);
            sortMapSets();
        }

        private void sortMapSets()
        {
            sets.Sort((x, y) => string.Compare(x.Metadata.Title, y.Metadata.Title, StringComparison.Ordinal));
        }

        public RealmMapSet GetRandom()
        {
            Random rnd = new Random();
            return sets[rnd.Next(sets.Count)];
        }

        public List<RealmMapSet> GetMapSets() => sets;

        public RealmMapSet FindSetFromMap(RealmMap map) => sets.Find(x => x.Maps.Contains(map));

        public string GetMapAudioPath(MapInfo map) => storage.GetFullPath($"maps{Path.DirectorySeparatorChar}{map.MapsetID}{Path.DirectorySeparatorChar}{map.GetAudioFile()}");
    }
}
