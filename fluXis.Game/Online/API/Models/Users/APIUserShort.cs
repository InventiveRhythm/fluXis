using System;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUserShort
{
    [JsonProperty("id")]
    public int ID { get; set; } = -1;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("country")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonIgnore]
    public CountryCode Country => GetCountryCode(CountryCode);

    [JsonProperty("displayname")]
    public string DisplayName { get; set; } = "";

    [JsonProperty("role")]
    public int Role { get; set; }

    public string GetAvatarUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/avatar/" + ID;
    public string GetBannerUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/banner/" + ID;

    public string GetName() => string.IsNullOrEmpty(DisplayName) ? Username : DisplayName;

    public static CountryCode GetCountryCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return Online.CountryCode.Unknown;

        return Enum.TryParse(code.ToUpper(), out CountryCode cc) ? cc : Online.CountryCode.Unknown;
    }

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
    public static APIUserShort AutoPlay => new() { ID = 0, Username = "AutoPlay" };

    public class APIUserSocials
    {
        [JsonProperty("twitter")]
        public string Twitter { get; set; } = string.Empty;

        [JsonProperty("twitch")]
        public string Twitch { get; set; } = string.Empty;

        [JsonProperty("youtube")]
        public string YouTube { get; set; } = string.Empty;

        [JsonProperty("discord")]
        public string Discord { get; set; } = string.Empty;
    }
}
