using Newtonsoft.Json;

namespace fluXis.Game.Updater.GitHub;

public class GitHubAsset
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
