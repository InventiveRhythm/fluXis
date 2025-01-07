using System;
using Newtonsoft.Json;

namespace fluXis.Updater.GitHub;

public class GitHubRelease
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("tag_name")]
    public string TagName { get; set; } = "";

    [JsonProperty("prerelease")]
    public bool PreRelease { get; set; }

    [JsonProperty("draft")]
    public bool Draft { get; set; }

    [JsonProperty("target_commitish")]
    public string TargetCommitish { get; set; }

    [JsonProperty("assets")]
    public GitHubAsset[] Assets { get; set; } = Array.Empty<GitHubAsset>();
}
