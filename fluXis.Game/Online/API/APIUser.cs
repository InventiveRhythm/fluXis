namespace fluXis.Game.Online.API;

public class APIUser : APIUserShort
{
    public string AboutMe;

    public static APIUser DummyUser(int id, string username = "Player")
    {
        return new APIUser
        {
            ID = id,
            Username = username,
            AboutMe = ""
        };
    }
}