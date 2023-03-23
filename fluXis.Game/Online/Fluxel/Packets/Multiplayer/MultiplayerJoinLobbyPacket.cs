using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerJoinLobbyPacket : Packet
{
    [JsonProperty("id")]
    public int LobbyId;

    [JsonProperty("password")]
    public string Password;

    public MultiplayerJoinLobbyPacket(int id, string password)
        : base(21)
    {
        LobbyId = id;
        Password = password;
    }
}
