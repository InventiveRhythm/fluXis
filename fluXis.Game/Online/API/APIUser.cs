using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIUser : APIUserShort
{
    [JsonProperty("aboutme")]
    public string AboutMe;

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

    public static string GetRole(int role)
    {
        return role switch
        {
            1 => "Featured Artist",
            2 => "Purifier",
            3 => "Moderator",
            4 => "Admin",
            5 => "Bot",
            _ => "User"
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
