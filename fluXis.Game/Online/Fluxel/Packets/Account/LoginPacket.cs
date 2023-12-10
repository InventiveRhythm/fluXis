using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class LoginPacket : Packet
{
    public override string ID => "account/login";

    [JsonProperty("token")]
    public string Token { get; init; }

    public LoginPacket(string token)
    {
        Token = token;
    }
}
