using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Account;

public class LoginPacket : IPacket
{
    public string ID => PacketIDs.LOGIN;

    #region Client2Server

    [JsonProperty("token")]
    public string? Token { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("user")]
    public IAPIUserShort? User { get; set; }

    #endregion

    public static LoginPacket CreateC2S(string token) => new() { Token = token };
    public static LoginPacket CreateS2C(IAPIUserShort user) => new() { User = user };
}
