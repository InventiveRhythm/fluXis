using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Multiplayer;

public class MultiLeavePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_LEAVE;

    #region Server2Client

    [JsonProperty("user")]
    public long UserID { get; set; }

    #endregion

    public static MultiLeavePacket CreateS2C(long userId) => new() { UserID = userId };
}
