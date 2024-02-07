using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Other;

public class MenuUpdate
{
    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
