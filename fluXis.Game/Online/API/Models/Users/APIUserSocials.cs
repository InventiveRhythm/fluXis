using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

#nullable enable

public class APIUserSocials
{
    [JsonProperty("twitter")]
    public string? Twitter { get; set; }

    [JsonProperty("twitch")]
    public string? Twitch { get; set; }

    [JsonProperty("youtube")]
    public string? YouTube { get; set; }

    [JsonProperty("discord")]
    public string? Discord { get; set; }
}
