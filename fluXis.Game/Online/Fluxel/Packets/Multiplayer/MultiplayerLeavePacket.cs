using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerLeavePacket : Packet
{
    public override int ID => 22;

    [JsonProperty("user")]
    public int UserID { get; init; }
}
