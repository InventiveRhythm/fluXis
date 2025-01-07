using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Account;

#nullable enable

public class LoginPacket : IPacket
{
    public string ID => PacketIDs.LOGIN;

    #region Client2Server

    [JsonProperty("token")]
    public string? Token { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("user")]
    public APIUser? User { get; set; }

    #endregion

    public static LoginPacket CreateC2S(string token) => new() { Token = token };
    public static LoginPacket CreateS2C(APIUser user) => new() { User = user };
}
