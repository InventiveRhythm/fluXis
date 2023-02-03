using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Map
{
    public class MapStore
    {
        private static readonly List<MapSet> map_sets = new List<MapSet>();
        private static Storage storage;

        public List<MapSet> MapSets => map_sets;

        public MapSet CurrentMapSet;

        public void LoadMaps(Storage stor)
        {
            storage = stor;

            if (storage.ExistsDirectory("maps"))
            {
                Logger.Log("Loading maps...");
                var dirs = storage.GetDirectories("maps");

                MD5 md5 = MD5.Create();

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

                            md5.Initialize();
                            map.MD5 = BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(storage.GetFullPath(chart)))).Replace("-", "").ToLower();
                            Logger.Log($"map: {map.ID} - {map.MD5}");

                            if (map.Validate())
                                mapset.AddMap(map);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to load mapset " + dirName);
                    }

                    if (mapset.Maps.Count > 0)
                        map_sets.Add(mapset);
                }
            }

            if (map_sets.Count == 0)
                Logger.Log("No maps found.");

            map_sets.Sort((a, b) =>
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

        public MapSet GetRandom()
        {
            Random rnd = new Random();
            return map_sets[rnd.Next(map_sets.Count)];
        }

        public List<MapSet> GetMapSets() => map_sets;

        public MapInfo GetMap(string mapsetID, string mapID)
        {
            var mapset = map_sets.Find(x => x.ID == mapsetID);
            return mapset?.Maps.Find(x => x.ID == mapID);
        }

        public MapSet GetMapSet(string mapsetID)
        {
            return map_sets.Find(x => x.ID == mapsetID);
        }

        public string GetMapAudioPath(MapInfo map) => storage.GetFullPath($"maps{Path.DirectorySeparatorChar}{map.MapsetID}{Path.DirectorySeparatorChar}{map.GetAudioFile()}");
    }
}
