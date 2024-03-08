using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public interface IAPIUserShort
{
    [JsonProperty("id")]
    long ID { get; set; }

    [JsonProperty("username")]
    string Username { get; set; }

    [JsonProperty("displayname")]
    string DisplayName { get; set; }

    [JsonProperty("country")]
    string? CountryCode { get; set; }
}
