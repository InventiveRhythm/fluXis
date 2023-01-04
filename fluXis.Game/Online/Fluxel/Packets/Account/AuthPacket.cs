using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Account
{
    public class AuthPacket : Packet
    {
        [JsonProperty("username")]
        public string Username;

        [JsonProperty("password")]
        public string Password;

        public AuthPacket(string username, string password)
            : base(0)
        {
            Username = username;
            Password = password;
        }
    }
}
