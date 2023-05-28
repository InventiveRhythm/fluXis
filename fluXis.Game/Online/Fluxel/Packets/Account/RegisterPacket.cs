using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class RegisterPacket : Packet
{
    [JsonProperty("username")]
    public string Username;

    [JsonProperty("password")]
    public string Password;

    [JsonProperty("email")]
    public string Email;

    public RegisterPacket()
        : base(2)
    {
    }
}
