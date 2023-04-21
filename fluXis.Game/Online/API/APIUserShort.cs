namespace fluXis.Game.Online.API;

public class APIUserShort
{
    public int ID;
    public string Username;

    public string AvatarUrl => APIConstants.APIUrl + "/assets/avatar/" + ID;
    public string BannerUrl => APIConstants.APIUrl + "/assets/banner/" + ID;

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
}
