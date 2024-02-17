using Newtonsoft.Json;

namespace fluXis.Game.Updater.GitHub;

public class GitHubRelease
{
    [JsonProperty("tag_name")]
    public string TagName { get; set; } = "";

    [JsonProperty("prerelease")]
    public bool PreRelease { get; set; }
}
