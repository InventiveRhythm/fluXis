using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Multiplayer;

public class MultiReadyPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_READY;

    [JsonProperty("ready")]
    public bool Ready { get; set; }

    #region Server2Client

    [JsonProperty("player_id")]
    public long PlayerID { get; set; }

    #endregion

    public static MultiReadyPacket CreateC2S(bool ready)
        => new() { Ready = ready };

    public static MultiReadyPacket CreateS2C(long playerID, bool ready)
        => new() { PlayerID = playerID, Ready = ready };
}
