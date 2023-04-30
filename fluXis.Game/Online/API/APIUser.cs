using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIUser : APIUserShort
{
    public string AboutMe;

    [JsonProperty("role")]
    public string Role;

    [JsonProperty("social")]
    public APIUserSocials Socials;

    public static APIUser DummyUser(int id, string username = "Player")
    {
        return new APIUser
        {
            ID = id,
            Username = username,
            AboutMe = ""
        };
    }

    public class APIUserSocials
    {
        public string Twitter;
        public string Twitch;
        public string Youtube;
        public string Discord;
    }
}
