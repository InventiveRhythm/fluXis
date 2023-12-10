using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class AuthPacket : Packet
{
    public override string ID => "account/auth";

    [JsonProperty("username")]
    public string Username { get; init; }

    [JsonProperty("password")]
    public string Password { get; init; }

    public AuthPacket(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
