using System.Collections.Generic;

namespace fluXis.Game.Map
{
    public class MapInfo
    {
        public string ID { get; set; }
        public MapMetadata Metadata { get; set; }
        public List<HitObjectInfo> HitObjects;
        public List<TimingPointInfo> TimingPoints;

        public int StartTime => HitObjects[0].Time;
        public int EndTime => HitObjects[^1].Time;

        public MapInfo(MapMetadata metadata)
        {
            ID = "";
            Metadata = metadata;
            HitObjects = new List<HitObjectInfo>();
            TimingPoints = new List<TimingPointInfo>();
        }

        public void Sort()
        {
            HitObjects.Sort((a, b) => a.Time.CompareTo(b.Time));
            TimingPoints.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
    }
}
