using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class LoginPacket : Packet
{
    [JsonProperty("token")]
    public string Token;

    public LoginPacket(string token)
        : base(1)
    {
        Token = token;
    }
}