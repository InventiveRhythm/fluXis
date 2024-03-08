using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public interface IAPIUserSocials
{
    [JsonProperty("twitter")]
    string Twitter { get; set; }

    [JsonProperty("twitch")]
    string Twitch { get; set; }

    [JsonProperty("youtube")]
    string YouTube { get; set; }

    [JsonProperty("discord")]
    string Discord { get; set; }
}
