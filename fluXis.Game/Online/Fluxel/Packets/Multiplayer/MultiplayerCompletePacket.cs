using fluXis.Game.Scoring;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

/// <summary>
/// Player has finished the map and is waiting for others to finish.
/// </summary>
public class MultiplayerCompletePacket : Packet
{
    public override string ID => "multi/complete";

    [JsonProperty("score")]
    public ScoreInfo Score { get; set; }

    public MultiplayerCompletePacket(ScoreInfo score)
    {
        Score = score;
    }
}
