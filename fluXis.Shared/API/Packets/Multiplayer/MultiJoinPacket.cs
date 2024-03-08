using fluXis.Shared.Components.Multi;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Multiplayer;

public class MultiJoinPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_JOIN;

    #region Client2Server

    [JsonProperty("lobbyId")]
    public long? LobbyID { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("lobby")]
    public IMultiplayerRoom? Room { get; init; }

    [JsonProperty("user")]
    public IAPIUserShort? Player { get; init; }

    [JsonIgnore]
    public bool JoinRequest => Room != null;

    [JsonIgnore]
    public bool SomeoneJoined => Player != null;

    #endregion

    public static MultiJoinPacket CreateC2SInitialJoin(long lobbyID, string? password = null)
        => new() { LobbyID = lobbyID, Password = password };

    public static MultiJoinPacket CreateS2CInitialJoin(IMultiplayerRoom room) => new() { Room = room };
    public static MultiJoinPacket CreateS2CUserJoin(IAPIUserShort player) => new() { Player = player };
}
