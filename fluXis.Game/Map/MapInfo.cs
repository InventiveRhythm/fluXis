using System.Collections.Generic;

namespace fluXis.Game.Map
{
    public class MapInfo
    {
        public string MapsetID { get; set; }
        public string ID { get; set; }
        public string AudioFile { get; set; }
        public string BackgroundFile { get; set; }
        public MapMetadata Metadata { get; set; }
        public List<HitObjectInfo> HitObjects;
        public List<TimingPointInfo> TimingPoints;
        public List<ScrollVelocityInfo> ScrollVelocities;

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
            ScrollVelocities?.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public string GetAudioFile()
        {
            return AudioFile ?? "audio.ogg";
        }

        public string GetBackgroundFile()
        {
            return BackgroundFile ?? "bg.png";
        }
    }
}
