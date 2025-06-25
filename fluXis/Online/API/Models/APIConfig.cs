using Newtonsoft.Json;

namespace fluXis.Online.API.Models;

public class APIConfig
{
    [JsonProperty("assets")]
    public string AssetsUrl { get; set; } = string.Empty;

    [JsonProperty("website")]
    public string WebsiteUrl { get; set; } = string.Empty;

    [JsonProperty("wiki")]
    public string WikiUrl { get; set; } = string.Empty;
}
