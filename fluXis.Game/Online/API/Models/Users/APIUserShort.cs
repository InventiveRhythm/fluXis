using System;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUserShortOld
{
    [JsonProperty("id")]
    public long ID { get; set; } = -1;

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

    public string GetAvatarUrl(APIEndpointConfig endpoint) => endpoint.AssetUrl + "/avatar/" + ID;

    public string GetName() => string.IsNullOrEmpty(DisplayName) ? Username : DisplayName;

    [JsonIgnore]
    public string NameWithApostrophe
    {
        get
        {
            var name = GetName();
            if (name.EndsWith("s") || name.EndsWith("z"))
                return name + "'";

            return name + "'s";
        }
    }

    public static CountryCode GetCountryCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return Online.CountryCode.Unknown;

        return Enum.TryParse(code.ToUpper(), out CountryCode cc) ? cc : Online.CountryCode.Unknown;
    }
}
