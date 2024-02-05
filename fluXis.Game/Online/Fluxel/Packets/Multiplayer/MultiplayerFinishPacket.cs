using System;
using System.Collections.Generic;
using fluXis.Game.Scoring;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerFinishPacket : Packet
{
    public override string ID => throw new NotImplementedException("This packet is not meant to be sent by the client.");

    [JsonProperty("scores")]
    public List<ScoreInfo> Scores { get; set; } = new();
}
