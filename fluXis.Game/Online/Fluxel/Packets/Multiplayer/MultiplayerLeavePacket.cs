using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerLeavePacket : Packet
{
    public override string ID => "multi/leave";

    [JsonProperty("user")]
    public int UserID { get; init; }
}
