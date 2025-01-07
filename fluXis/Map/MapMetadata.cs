using Newtonsoft.Json;

namespace fluXis.Map;

public class MapMetadata
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("mapper")]
    public string Mapper { get; set; } = string.Empty;

    [JsonProperty("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonProperty("source")]
    public string AudioSource { get; set; } = string.Empty;

    [JsonProperty("bg-source")]
    public string BackgroundSource { get; set; } = string.Empty;

    [JsonProperty("cover-source")]
    public string CoverSource { get; set; } = string.Empty;

    [JsonProperty("tags")]
    public string Tags { get; set; } = string.Empty;

    [JsonProperty("previewtime")]
    public int PreviewTime { get; set; }
}
