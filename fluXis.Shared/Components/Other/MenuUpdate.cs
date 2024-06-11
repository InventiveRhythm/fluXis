using Newtonsoft.Json;

namespace fluXis.Shared.Components.Other;

public class MenuUpdate
{
    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
