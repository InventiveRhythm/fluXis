using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerRoomSettings
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("password")]
    public bool HasPassword { get; set; }
}
