using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Json;

public class SkinInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("creator")]
    public string Creator { get; set; } = "SKIN CREATOR";

    [JsonProperty("accent")]
    public string AccentHex { get; set; } = string.Empty;

    [JsonIgnore]
    public string Path { get; set; } = string.Empty;

    [JsonIgnore]
    public bool SteamWorkshop { get; set; }

    [CanBeNull]
    [JsonIgnore]
    public Texture IconTexture { get; set; }
}
