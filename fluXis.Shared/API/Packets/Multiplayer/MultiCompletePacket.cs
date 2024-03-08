using fluXis.Shared.Scoring;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Multiplayer;

public class MultiCompletePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_COMPLETE;

    [JsonProperty("score")]
    public ScoreInfo? Score { get; set; }

    public static MultiCompletePacket CreateC2S(ScoreInfo score)
        => new() { Score = score };
}
