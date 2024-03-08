using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Account;

public class AuthPacket : IPacket
{
    public string ID => PacketIDs.AUTH;

    #region Client2Server

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("token")]
    public string? Token { get; set; }

    #endregion

    public static AuthPacket CreateC2S(string username, string password) => new() { Username = username, Password = password };
    public static AuthPacket CreateS2C(string token) => new() { Token = token };
}
