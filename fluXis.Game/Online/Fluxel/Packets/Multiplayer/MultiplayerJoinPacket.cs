using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerJoinPacket : Packet
{
    [JsonProperty("lobbyId")]
    public int LobbyId = -1;

    [JsonProperty("password")]
    public string Password = string.Empty;

    public MultiplayerJoinPacket()
        : base(21)
    {
    }
}
