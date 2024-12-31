using fluXis.Online.Fluxel;
using fluXis.Scoring;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Multiplayer;

#nullable enable

public class MultiCompletePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_COMPLETE;

    [JsonProperty("score")]
    public ScoreInfo? Score { get; set; }

    public static MultiCompletePacket CreateC2S(ScoreInfo score)
        => new() { Score = score };
}
