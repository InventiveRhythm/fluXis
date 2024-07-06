using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Maps;

public class APIMapSet
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("creator")]
    public APIUser Creator { get; set; } = APIUser.CreateUnknown(0);

    [JsonProperty("maps")]
    public List<APIMap> Maps { get; set; } = new();

    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("artist")]
    public string Artist { get; set; } = "";

    [JsonProperty("source")]
    public string Source { get; set; } = "";

    [JsonProperty("tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [JsonProperty("flags")]
    public MapSetFlag Flags { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("submitted")]
    public long DateSubmitted { get; set; }

    [JsonProperty("updated")]
    public long LastUpdated { get; set; }
}
