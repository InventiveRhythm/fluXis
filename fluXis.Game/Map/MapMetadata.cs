namespace fluXis.Game.Map
{
    public class MapMetadata
    {
        public string Title;
        public string Artist;
        public string Mapper;
        public string Difficulty { get; set; }
        public string Source;
        public string Tags;
        public int PreviewTime;

        public MapMetadata()
        {
            Title = "";
            Artist = "";
            Mapper = "";
            Difficulty = "";
            Source = "";
            Tags = "";
            PreviewTime = 0;
        }
    }
}
