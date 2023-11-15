using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerCreateRoomPacket : Packet
{
    public override int ID => 20;

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("password")]
    public string Password { get; init; }

    [JsonProperty("max")]
    public int MaxPlayers { get; init; }

    public MultiplayerCreateRoomPacket(string name, string password, int maxPlayers)
    {
        Name = name;
        Password = password;
        MaxPlayers = maxPlayers;
    }
}
