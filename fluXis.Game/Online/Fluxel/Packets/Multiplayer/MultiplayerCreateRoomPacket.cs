using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerCreateRoomPacket : Packet
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("password")]
    public string Password;

    [JsonProperty("max")]
    public int MaxPlayers;

    public MultiplayerCreateRoomPacket(string name, string password, int maxPlayers)
        : base(20)
    {
        Name = name;
        Password = password;
        MaxPlayers = maxPlayers;
    }
}
