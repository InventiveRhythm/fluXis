namespace fluXis.Game.Map;

public class MapMetadata
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Mapper { get; set; }
    public string Difficulty { get; set; }
    public string Source { get; set; }
    public string Tags { get; set; }
    public int PreviewTime { get; set; }

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