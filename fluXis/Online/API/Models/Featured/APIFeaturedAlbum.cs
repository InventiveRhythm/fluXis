using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Featured;

public class APIFeaturedAlbum
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("release")]
    public AlbumRelease Release { get; set; } = null!;

    [JsonProperty("colors")]
    public AlbumColors Colors { get; set; } = null!;

    [JsonProperty("tracks")]
    public List<APIFeaturedTrack> Tracks { get; set; } = new();

    public class AlbumRelease
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("day")]
        public int Day { get; set; }
    }

    public class AlbumColors
    {
        [JsonProperty("accent")]
        public string Accent { get; set; } = "#ffffff";

        [JsonProperty("text")]
        public string Text { get; set; } = "#ffffff";

        [JsonProperty("text2")]
        public string Text2 { get; set; } = "#ffffff";

        [JsonProperty("bg")]
        public string Background { get; set; } = "#ffffff";

        [JsonProperty("bg2")]
        public string Background2 { get; set; } = "#ffffff";
    }
}
