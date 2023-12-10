using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account;

public class RegisterPacket : Packet
{
    public override string ID => "account/register";

    [JsonProperty("username")]
    public string Username { get; init; }

    [JsonProperty("password")]
    public string Password { get; init; }

    [JsonProperty("email")]
    public string Email { get; init; }

    public RegisterPacket(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }
}
