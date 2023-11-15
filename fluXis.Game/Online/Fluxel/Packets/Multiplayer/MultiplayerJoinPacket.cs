using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerJoinPacket : Packet
{
    public override int ID => 21;

    [JsonProperty("lobbyId")]
    public int LobbyId { get; init; }

    [JsonProperty("password")]
    public string Password { get; init; }

    public MultiplayerJoinPacket(int lobbyId, string password)
    {
        LobbyId = lobbyId;
        Password = password;
    }

    // s2c

    [JsonProperty("lobby")]
    public MultiplayerRoom Room { get; init; }

    [JsonProperty("user")]
    public APIUserShort Player { get; init; }

    public bool JoinRequest => Room != null;
    public bool SomeoneJoined => Player != null;
}
