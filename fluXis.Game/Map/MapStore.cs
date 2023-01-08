using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map
{
    public class MapStore
    {
        private static readonly List<MapSet> mapsets = new List<MapSet>();
        private static Storage storage;

        public void LoadMaps(Storage stor)
        {
            storage = stor;

            if (storage.ExistsDirectory("maps"))
            {
                Logger.Log("Loading maps...");
                var dirs = storage.GetDirectories("maps");

                foreach (var dir in dirs)
                {
                    string dirName = dir.Split(Path.DirectorySeparatorChar).Last();
                    MapSet mapset = new MapSet(dirName);

                    try
                    {
                        var charts = storage.GetFiles(dir, "*.fsc");

                        foreach (var chart in charts)
                        {
                            MapInfo map = JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText(storage.GetFullPath(chart)));
                            map.MapsetID = dirName;
                            map.ID = Path.GetFileNameWithoutExtension(chart);
                            map.Sort();

                            if (map.Validate())
                                mapset.AddMap(map);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to load mapset " + dirName);
                    }

                    if (mapset.Maps.Count > 0)
                        mapsets.Add(mapset);
                }
            }
            else
            {
                Logger.Log("No maps found.");
            }

            mapsets.Sort((a, b) =>
            {
                string aTitle = a.Title.ToLower();
                string bTitle = b.Title.ToLower();

                if (aTitle == bTitle)
                {
                    string aArtist = a.Artist.ToLower();
                    string bArtist = b.Artist.ToLower();

                    return string.Compare(aArtist, bArtist, StringComparison.Ordinal);
                }

                return string.Compare(aTitle, bTitle, StringComparison.Ordinal);
            });
        }

        public List<MapSet> GetMapSets() => mapsets;

        public MapInfo GetMap(string mapsetID, string mapID)
        {
            var mapset = mapsets.Find(x => x.ID == mapsetID);
            return mapset?.Maps.Find(x => x.ID == mapID);
        }

        public string GetMapAudioPath(MapInfo map) => storage.GetFullPath($"maps{Path.DirectorySeparatorChar}{map.MapsetID}{Path.DirectorySeparatorChar}{map.GetAudioFile()}");
    }
}
