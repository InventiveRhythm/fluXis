using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.Multiplayer;

#nullable enable

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

    [JsonProperty("participant")]
    public IMultiplayerParticipant? Participant { get; init; }

    [JsonIgnore]
    public bool JoinRequest => Room != null;

    [JsonIgnore]
    public bool SomeoneJoined => Participant != null;

    #endregion

    public static MultiJoinPacket CreateC2SInitialJoin(long lobbyID, string? password = null)
        => new() { LobbyID = lobbyID, Password = password };

    public static MultiJoinPacket CreateS2CInitialJoin(IMultiplayerRoom room) => new() { Room = room };
    public static MultiJoinPacket CreateS2CUserJoin(IMultiplayerParticipant participant) => new() { Participant = participant };
}
