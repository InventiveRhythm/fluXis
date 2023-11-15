using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class LoginPacket : Packet
{
    public override int ID => 1;

    [JsonProperty("token")]
    public string Token { get; init; }

    public LoginPacket(string token)
    {
        Token = token;
    }
}
