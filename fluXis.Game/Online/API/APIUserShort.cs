namespace fluXis.Game.Online.API;

public class APIUserShort
{
    public int ID;
    public string Username;

    public string GetAvatarUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/avatar/" + ID;
    public string GetBannerUrl(APIEndpointConfig endpoint) => endpoint.APIUrl + "/assets/banner/" + ID;

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
}
