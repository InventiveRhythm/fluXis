using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Account;

public class RegisterPacket : IPacket
{
    public string ID => PacketIDs.REGISTER;

    #region Client2Server

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("token")]
    public string Token { get; set; } = null!;

    [JsonProperty("user")]
    public APIUserShort User { get; set; } = null!;

    #endregion

    public static RegisterPacket CreateC2S(string username, string email, string password)
        => new() { Username = username, Email = email, Password = password };

    public static RegisterPacket CreateS2C(string token, APIUserShort user) => new() { Token = token, User = user };
}
