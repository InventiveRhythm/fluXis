using System.Collections.Generic;
using System.IO;

namespace fluXis.Game.Map
{
    public class MapSet
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Source;
        public string Tags;
        public List<MapInfo> Maps { get; set; }

        public MapSet(string id)
        {
            ID = id;
            Maps = new List<MapInfo>();
        }

        public void AddMap(MapInfo map)
        {
            // take the mapset's metadata from the first map
            if (Maps.Count == 0)
            {
                Title = map.Metadata.Title;
                Artist = map.Metadata.Artist;
                Source = map.Metadata.Source;
                Tags = map.Metadata.Tags;
            }

            Maps.Add(map);
        }

        public string GetBackgroundPath()
        {
            if (Maps.Count > 0)
                return ID + Path.DirectorySeparatorChar + Maps[0].GetBackgroundFile();
            else
                return null;
        }

        public MapInfo GetFirstMap()
        {
            return Maps[0];
        }
    }
}
