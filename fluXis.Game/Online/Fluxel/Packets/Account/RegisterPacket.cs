using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class RegisterPacket : Packet
{
    [JsonProperty("username")]
    public string Username;

    [JsonProperty("password")]
    public string Password;

    public RegisterPacket(string username, string password)
        : base(2)
    {
        Username = username;
        Password = password;
    }
}
