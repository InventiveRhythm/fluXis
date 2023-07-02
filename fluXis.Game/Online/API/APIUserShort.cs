using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIUserShort
{
    [JsonProperty("id")]
    public int ID;

    [JsonProperty("username")]
    public string Username;

    [JsonProperty("country")]
    public string CountryCode;

    [JsonProperty("role")]
    public int Role;

    public string GetAvatarUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/avatar/" + ID;
    public string GetBannerUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/banner/" + ID;

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
}
