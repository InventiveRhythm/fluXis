using Newtonsoft.Json;

namespace fluXis.Shared.Components.Multi;

public interface IMultiplayerRoomSettings
{
    [JsonProperty("name")]
    string Name { get; set; }

    [JsonProperty("password")]
    bool HasPassword { get; }
}
