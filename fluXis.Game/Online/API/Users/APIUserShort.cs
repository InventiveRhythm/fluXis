using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Users;

public class APIUserShort
{
    [JsonProperty("id")]
    public int ID { get; set; } = -1;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("country")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonProperty("role")]
    public int Role { get; set; }

    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("ptr")]
    public double PotentialRating { get; set; }

    public string GetAvatarUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/avatar/" + ID;
    public string GetBannerUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/banner/" + ID;

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
}
