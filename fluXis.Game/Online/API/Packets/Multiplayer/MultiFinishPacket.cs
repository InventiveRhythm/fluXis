using System.Collections.Generic;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Scoring;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.Multiplayer;

public class MultiFinishPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_FINISH;

    [JsonProperty("scores")]
    public List<ScoreInfo> Scores { get; set; }

    public MultiFinishPacket(List<ScoreInfo> scores)
    {
        Scores = scores;
    }
}
