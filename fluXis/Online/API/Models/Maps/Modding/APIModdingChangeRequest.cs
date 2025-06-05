using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps.Modding;

public class APIModdingChangeRequest
{
    [JsonProperty("start")]
    public double StartTime { get; set; }

    [JsonProperty("end")]
    public double? EndTime { get; set; }

    [JsonProperty("content")]
    public string CommentContent { get; set; } = string.Empty;

    [JsonProperty("color")]
    public string HexColor { get; set; } = string.Empty;
}
